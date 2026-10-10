using System;
using UnityEngine;

public class LaboratoryNotificationDot : NotificationDot
{
    [SerializeField] private LaboratoryManager labManager;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private JournalManager journalManager;

    private Action<int> onCoinsChangedHandler;
    private Action<CropData, int> onCropInventoryChangedHandler;
    private Action<CropData> onCropStateChangedHandler;
    private Action onLabChangedHandler;

    private void Awake()
    {
        onCoinsChangedHandler = _ => Refresh();
        onCropInventoryChangedHandler = (_, __) => Refresh();
        onCropStateChangedHandler = _ => Refresh();
        onLabChangedHandler = Refresh;
    }

    private void OnEnable()
    {
        resourceManager.OnCoinsChanged += onCoinsChangedHandler;
        resourceManager.OnCropChanged += onCropInventoryChangedHandler;
        labManager.OnCropResearched += onCropStateChangedHandler;
        labManager.OnCropAutomated += onCropStateChangedHandler;
        labManager.OnLabActionStarted += onLabChangedHandler;
        journalManager.OnChapter1Claimed += onLabChangedHandler;
    }

    private void OnDisable()
    {
        resourceManager.OnCoinsChanged -= onCoinsChangedHandler;
        resourceManager.OnCropChanged -= onCropInventoryChangedHandler;
        labManager.OnCropResearched -= onCropStateChangedHandler;
        labManager.OnCropAutomated -= onCropStateChangedHandler;
        labManager.OnLabActionStarted -= onLabChangedHandler;
        journalManager.OnChapter1Claimed -= onLabChangedHandler;
    }

    protected override bool IsUnlocked()
    {
        return journalManager.IsChapter1Claimed();
    }

    protected override bool ShouldShow()
    {
        return labManager.HasAvailableAction();
    }
}