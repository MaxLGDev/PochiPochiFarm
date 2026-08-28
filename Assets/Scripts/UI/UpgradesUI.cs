using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesUI : MonoBehaviour
{
    private Action<int> onCoinsChangedHandler;
    private Action<CropData> onRequestedCropUnlockedHandler;
    private Action<CropData> onRequestedCropResearchedHandler;
    private Action<CropData> onRequestedCropAutomatedHandler;
    
    [SerializeField] private GridManager gridManager;
    [SerializeField] private LaboratoryManager laboratoryManager;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private UpgradeManager upgradeManager;
    
    [SerializeField] private List<UpgradeNodeUI> upgradeNodes;

    private void Awake()
    {
        onCoinsChangedHandler = (amount) => RefreshAll();
        onRequestedCropUnlockedHandler = (crop) => RefreshAll();
        onRequestedCropResearchedHandler = (crop) => RefreshAll();
        onRequestedCropAutomatedHandler = (crop) => RefreshAll();
    }

    private void OnEnable()
    {
        gridManager.OnCropUnlocked += onRequestedCropUnlockedHandler;
        laboratoryManager.OnRequestedCropResearched += onRequestedCropResearchedHandler;
        laboratoryManager.OnRequestedCropAutomated += onRequestedCropAutomatedHandler;
        resourceManager.OnCoinsChanged += onCoinsChangedHandler;
        upgradeManager.OnUpgradeUnlocked += RefreshAll;
    }

    private void OnDisable()
    {
        gridManager.OnCropUnlocked -= onRequestedCropUnlockedHandler;
        laboratoryManager.OnRequestedCropResearched -= onRequestedCropResearchedHandler;
        laboratoryManager.OnRequestedCropAutomated -= onRequestedCropAutomatedHandler;
        resourceManager.OnCoinsChanged -= onCoinsChangedHandler;
        upgradeManager.OnUpgradeUnlocked -= RefreshAll;
    }

    private void RefreshAll()
    {
        foreach (UpgradeNodeUI node in upgradeNodes)
            node.Refresh();
    }
}
