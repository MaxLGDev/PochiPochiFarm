using UnityEngine;
using System;

public class UpgradesNotificationDot : NotificationDot
{
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private LaboratoryManager labManager;
    [SerializeField] private JournalManager journalManager;

    private Action<int> onCoinsChangedHandler;
    private Action<UpgradeData> onUpgradeUnlockedHandler;
    private Action<CropData> onCropStateChangedHandler;
    private Action onChapter1ClaimedHandler;

    private void Awake()
    {
        onCoinsChangedHandler = _ => Refresh();
        onUpgradeUnlockedHandler = _ => Refresh();
        onCropStateChangedHandler = _ => Refresh();
        onChapter1ClaimedHandler = Refresh;
    }

    private void OnEnable()
    {
        resourceManager.OnCoinsChanged += onCoinsChangedHandler;
        upgradeManager.OnUpgradeUnlocked += onUpgradeUnlockedHandler;
        gridManager.OnCropUnlocked += onCropStateChangedHandler;
        labManager.OnRequestedCropResearched += onCropStateChangedHandler;
        labManager.OnRequestedCropAutomated += onCropStateChangedHandler;
        journalManager.OnChapter1Claimed += onChapter1ClaimedHandler;
    }

    private void OnDisable()
    {
        resourceManager.OnCoinsChanged -= onCoinsChangedHandler;
        upgradeManager.OnUpgradeUnlocked -= onUpgradeUnlockedHandler;
        gridManager.OnCropUnlocked -= onCropStateChangedHandler;
        labManager.OnRequestedCropResearched -= onCropStateChangedHandler;
        labManager.OnRequestedCropAutomated -= onCropStateChangedHandler;
        journalManager.OnChapter1Claimed -= onChapter1ClaimedHandler;
    }

    protected override bool IsUnlocked()
    {
        return journalManager.IsChapter1Claimed();
    }

    protected override bool ShouldShow()
    {
        return upgradeManager.HasAffordableUpgrade();
    }
}
