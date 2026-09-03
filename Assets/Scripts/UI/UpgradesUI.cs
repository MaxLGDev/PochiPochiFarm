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
    private Action<UpgradeData> onNodeClickedHandler;
    
    [SerializeField] private GridManager gridManager;
    [SerializeField] private LaboratoryManager laboratoryManager;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private JournalManager journalManager;
    
    [SerializeField] private List<UpgradeNodeUI> upgradeNodes;
    [SerializeField] private GameObject upgradesPanel;
    [SerializeField] private Button upgradesButton;

    private bool upgradesUnlocked = false;

    private void Awake()
    {
        onCoinsChangedHandler = (amount) => RefreshAll();
        onRequestedCropUnlockedHandler = (crop) => RefreshAll();
        onRequestedCropResearchedHandler = (crop) => RefreshAll();
        onRequestedCropAutomatedHandler = (crop) => RefreshAll();

        onNodeClickedHandler = (upgradeData) =>
        {
            upgradeManager.UnlockUpgrade(upgradeData);
            RefreshAll();
        };
    }

    private void Start()
    {
        upgradesPanel.SetActive(false);
        upgradesButton.interactable = false;

        RefreshAll();
    }

    private void OnEnable()
    {
        foreach (var node in upgradeNodes)
            node.OnNodeClicked += onNodeClickedHandler;
        
        journalManager.OnChapter1Claimed += HandleJournalChapter1Claimed;
        gridManager.OnCropUnlocked += onRequestedCropUnlockedHandler;
        laboratoryManager.OnRequestedCropResearched += onRequestedCropResearchedHandler;
        laboratoryManager.OnRequestedCropAutomated += onRequestedCropAutomatedHandler;
        resourceManager.OnCoinsChanged += onCoinsChangedHandler;
        upgradeManager.OnUpgradeUnlocked += RefreshAll;
    }

    private void OnDisable()
    {
        foreach(var node in upgradeNodes)
            node.OnNodeClicked -= onNodeClickedHandler;
        
        journalManager.OnChapter1Claimed -= HandleJournalChapter1Claimed;
        gridManager.OnCropUnlocked -= onRequestedCropUnlockedHandler;
        laboratoryManager.OnRequestedCropResearched -= onRequestedCropResearchedHandler;
        laboratoryManager.OnRequestedCropAutomated -= onRequestedCropAutomatedHandler;
        resourceManager.OnCoinsChanged -= onCoinsChangedHandler;
        upgradeManager.OnUpgradeUnlocked -= RefreshAll;
    }

    private void RefreshAll()
    {
        foreach (var node in upgradeNodes)
        {
            node.Refresh(upgradeManager.GetUpgradeState(node.UpgradeDataSo));
        }
    }

   private UpgradeNodeUI FindNodeFor(UpgradeData data)
   {
       foreach (var node in upgradeNodes)
       {
           if (node.UpgradeDataSo == data)
               return node;
       }

       return null;
   }

 
    private void HandleJournalChapter1Claimed()
    {
        if (upgradesUnlocked)
            return;
        
        upgradesUnlocked = true;
        upgradesButton.interactable = true;
    }

    public void ToggleUpgradesPanel() => upgradesPanel.SetActive(!upgradesPanel.activeSelf);
}
