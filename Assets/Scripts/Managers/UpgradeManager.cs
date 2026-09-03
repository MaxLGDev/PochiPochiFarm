using UnityEngine;
using System.Collections.Generic;
using System;

public enum UpgradeState
{
    Locked,
    Available,
    Bought
}

public class UpgradeManager : MonoBehaviour
{
    public event Action OnUpgradeUnlocked;
    
    [SerializeField] private GridManager gridManager;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private LaboratoryManager laboratoryManager;

    private Dictionary<UpgradeData, bool> allUpgrades;
    [SerializeField] private List<UpgradeData> allUpgradesList;
    public IReadOnlyList<UpgradeData> AllUpgrades => allUpgradesList;

    private void Awake()
    {
        allUpgrades = new Dictionary<UpgradeData, bool>();
        
        foreach (var upgrade in allUpgradesList)
            allUpgrades[upgrade] = false;
    }

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
        OnUpgradeUnlocked?.Invoke();
    }

    private bool AreRequirementsMet(UpgradeData data)
    {
        bool cropRequirementMet = false;

        if (data.PreviousUpgrade != null && !allUpgrades[data.PreviousUpgrade])
            return false;
        
        if (data.TargetCrop == null)
            cropRequirementMet = true;
        
        if (data.TargetCrop != null && data.CropState != RequiredCropState.None)
        {
            cropRequirementMet = data.CropState switch
            {
                RequiredCropState.Unlocked => gridManager.IsCropUnlocked(data.TargetCrop),
                RequiredCropState.Researched => laboratoryManager.IsCropResearched(data.TargetCrop),
                RequiredCropState.Automated => laboratoryManager.IsCropAutomated(data.TargetCrop),
                _ => cropRequirementMet
            };
        }
        
        return cropRequirementMet;
    }

    public UpgradeState GetUpgradeState(UpgradeData data)
    {
        if (allUpgrades[data])
            return UpgradeState.Bought;

        return AreRequirementsMet(data) ? UpgradeState.Available : UpgradeState.Locked;
    }
}
