using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private JournalManager journalManager;
    
    [SerializeField] private List<UpgradeNodeUI> upgradeNodes;
    [SerializeField] private GameObject upgradesPanel;
    [SerializeField] private Button upgradesButton;
    
    [SerializeField] private UpgradeNodeUI[] nodesUI;

    private bool upgradesUnlocked = false;

    private void Awake()
    {
        onCoinsChangedHandler = (amount) => RefreshAll();
        onRequestedCropUnlockedHandler = (crop) => RefreshAll();
        onRequestedCropResearchedHandler = (crop) => RefreshAll();
        onRequestedCropAutomatedHandler = (crop) => RefreshAll();
        
        nodesUI = GetComponentsInChildren<UpgradeNodeUI>();
    }

    private void Start()
    {
        upgradesPanel.SetActive(false);
        upgradesButton.interactable = false;
    }

    private void OnEnable()
    {
        journalManager.OnChapter1Claimed += HandleJournalChapter1Claimed;
        gridManager.OnCropUnlocked += onRequestedCropUnlockedHandler;
        laboratoryManager.OnRequestedCropResearched += onRequestedCropResearchedHandler;
        laboratoryManager.OnRequestedCropAutomated += onRequestedCropAutomatedHandler;
        resourceManager.OnCoinsChanged += onCoinsChangedHandler;
        upgradeManager.OnUpgradeUnlocked += RefreshAll;
    }

    private void OnDisable()
    {
        journalManager.OnChapter1Claimed -= HandleJournalChapter1Claimed;
        gridManager.OnCropUnlocked -= onRequestedCropUnlockedHandler;
        laboratoryManager.OnRequestedCropResearched -= onRequestedCropResearchedHandler;
        laboratoryManager.OnRequestedCropAutomated -= onRequestedCropAutomatedHandler;
        resourceManager.OnCoinsChanged -= onCoinsChangedHandler;
        upgradeManager.OnUpgradeUnlocked -= RefreshAll;
    }

    private void RefreshAll()
    {
        foreach (UpgradeNodeUI node in upgradeNodes)
        {
            node.Refresh();
        }
    }
    
    private void HandleJournalChapter1Claimed()
    {
        Debug.Log(upgradesButton.interactable);
        
        if (upgradesUnlocked)
            return;

        if (!upgradesUnlocked)
        {
            upgradesUnlocked = true;
            upgradesButton.interactable = true;
        }
    }
    
    public void ToggleUpgradesPanel() => upgradesPanel.SetActive(!upgradesPanel.activeSelf);
}
