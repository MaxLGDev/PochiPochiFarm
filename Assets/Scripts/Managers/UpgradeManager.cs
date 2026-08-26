using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private LaboratoryManager laboratoryManager;

    private Dictionary<UpgradeData, bool> allUpgrades;
    [SerializeField] private List<UpgradeData> allUpgradesList;

    private void Awake()
    {
        allUpgrades = new Dictionary<UpgradeData, bool>();
        
        foreach (UpgradeData upgrade in allUpgradesList)
            allUpgrades[upgrade] = false;
    }

    public void UnlockUpgrade(UpgradeData upgradeData)
    {
        bool cropRequirementMet = false;
        
        if (upgradeData.PreviousUpgrade != null && !allUpgrades[upgradeData.PreviousUpgrade])
            return;

        if (upgradeData.TargetCrop == null)
            cropRequirementMet = true;
        
        if (upgradeData.TargetCrop != null && upgradeData.CropState != RequiredCropState.None)
        {
            cropRequirementMet = upgradeData.CropState switch
            {
                RequiredCropState.Unlocked => gridManager.IsCropUnlocked(upgradeData.TargetCrop),
                RequiredCropState.Researched => laboratoryManager.IsCropResearched(upgradeData.TargetCrop),
                RequiredCropState.Automated => laboratoryManager.IsCropAutomated(upgradeData.TargetCrop),
                _ => cropRequirementMet
            };
        }

        if (!cropRequirementMet)
            return;

        if (!resourceManager.TrySpendCoins(upgradeData.UnlockCost))
        {
            Debug.Log("Not enough coins");
            return;
        }
        
        allUpgrades[upgradeData] = true;
    }
}
