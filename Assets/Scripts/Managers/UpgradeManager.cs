using System;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeState
{
    Locked,
    Available,
    Bought
}

// ## Known and accepted coupling
// UpgradeManager is the hub for upgrade effects. ResourceManager and LaboratoryManager
//     reference it (to subscribe to OnUpgradeUnlocked) and it references them back
//     (to spend coins / read crop state). Intentional: each manager applies its own effects.
//     Do not "fix" this without a real bug, it would need an interface or mediator.

public class UpgradeManager : MonoBehaviour
{
    // --- Events ---
    public event Action<UpgradeData> OnUpgradeUnlocked;

    // --- References ---
    [SerializeField] private GridManager gridManager;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private LaboratoryManager laboratoryManager;

    // --- Upgrade Data ---
    [SerializeField] private List<UpgradeData> allUpgradesList;

    // --- Runtime State ---
    private Dictionary<UpgradeData, bool> allUpgrades;

    public IReadOnlyList<UpgradeData> AllUpgrades => allUpgradesList;


    // ==============================
    // Unity Lifecycle
    // ==============================

    private void Awake()
    {
        allUpgrades = new Dictionary<UpgradeData, bool>();

        foreach (UpgradeData upgrade in allUpgradesList)
        {
            allUpgrades[upgrade] = false;
        }
    }

    public (int completed, int total) GetTotalUpgradesProgress()
    {
        int completed = 0;
        int total = allUpgradesList.Count;

        foreach (UpgradeData upgrade in allUpgradesList)
        {
            if (allUpgrades[upgrade])
                completed++;
        }

        return (completed, total);
    }

    // ==============================
    // Upgrade Management
    // ==============================

    public void UnlockUpgrade(UpgradeData upgradeData)
    {
        if (GetUpgradeState(upgradeData) != UpgradeState.Available)
            return;

        if (!resourceManager.TrySpendCoins(upgradeData.UnlockCost))
        {
            Debug.Log("Not enough coins");
            return;
        }

        allUpgrades[upgradeData] = true;
        OnUpgradeUnlocked?.Invoke(upgradeData);
        SoundManager.Instance.PlaySFX("UpgradeBought");
    }

    public UpgradeState GetUpgradeState(UpgradeData data)
    {
        if (allUpgrades[data])
            return UpgradeState.Bought;

        return AreRequirementsMet(data)
            ? UpgradeState.Available
            : UpgradeState.Locked;
    }


    // ==============================
    // Requirement Checks
    // ==============================

    private bool AreRequirementsMet(UpgradeData data)
    {
        if (data.PreviousUpgrade != null && !allUpgrades[data.PreviousUpgrade])
            return false;

        return IsCropRequirementMet(data);
    }

    public bool IsCropRequirementMet(UpgradeData data)
    {
        if (data.TargetCrop == null)
            return true;

        return data.CropState switch
        {
            RequiredCropState.Unlocked => gridManager.IsCropUnlocked(data.TargetCrop),
            RequiredCropState.Researched => laboratoryManager.IsCropResearched(data.TargetCrop),
            RequiredCropState.Automated => laboratoryManager.IsCropAutomated(data.TargetCrop),
            _ => false
        };
    }

    public UpgradesSaveData CaptureState()
    {
        UpgradesSaveData data = new UpgradesSaveData();

        foreach (KeyValuePair<UpgradeData, bool> pair in allUpgrades)
        {
            if(pair.Value)
                data.boughtUpgradeIds.Add(pair.Key.name);
        }

        return data;
    }

    public void ApplyState(UpgradesSaveData data)
    {
        List<UpgradeData> restored = new List<UpgradeData>();

        foreach (string id in data.boughtUpgradeIds)
        {
            UpgradeData upgrade = allUpgradesList.Find(u => u.name == id);
            if (upgrade == null)
            {
                Debug.LogWarning($"Save refers to unknown upgrade '{id}'");
                continue;
            }

            allUpgrades[upgrade] = true;
            restored.Add(upgrade);
        }

        foreach (UpgradeData upgrade in restored)
            OnUpgradeUnlocked?.Invoke(upgrade);

    }

    /// <summary>
    /// True if at least one upgrade is available and the player can pay for it.
    /// </summary>
    public bool HasAffordableUpgrade()
    {
        foreach (UpgradeData upgrade in allUpgradesList)
        {
            if(GetUpgradeState((upgrade)) == UpgradeState.Available && resourceManager.HasEnoughCoins(upgrade.UnlockCost))
            {
                return true;
            }
        }

        return false;
    }
}