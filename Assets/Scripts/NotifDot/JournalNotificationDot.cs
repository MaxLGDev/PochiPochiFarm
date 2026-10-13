using UnityEngine;
using System;

public class JournalNotificationDot : NotificationDot
{
    [SerializeField] private JournalManager journalManager;

    private Action<ObjData> onObjectiveCompleteHandler;
    private Action onChangedHandler;

    private void Awake()
    {
        onObjectiveCompleteHandler = _ => Refresh();
        onChangedHandler = Refresh;
    }

    private void OnEnable()
    {
        journalManager.OnObjectiveCompleted += onObjectiveCompleteHandler;
        journalManager.OnObjectiveClaimed += onChangedHandler;
    }

    private void OnDisable()
    {
        journalManager.OnObjectiveCompleted -= onObjectiveCompleteHandler;
        journalManager.OnObjectiveClaimed -= onChangedHandler;
    }

    protected override bool ShouldShow()
    {
        return journalManager.HasClaimableReward();
    }
}
