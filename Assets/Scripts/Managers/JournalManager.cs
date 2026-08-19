using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public enum ObjectiveType
{
    ClickCount,
    CoinsEarned,
    UnlockCrop,
    ResearchCrop,
    AutomateCrop,
    WaterRefilled,
    CropGathered
}

class ObjState
{
    public int Progress { get; private set; }
    public bool IsComplete { get; private set; }
    public bool IsClaimed { get; private set; }

    public void IncreaseProgress(int amount, int target)
    {
        Progress += amount;

        if(Progress >= target)
            SetObjectiveComplete();
    }

    public void SetObjectiveComplete() => IsComplete = true;
    public void SetObjectiveClaimed()
    {
        if (!IsComplete)
            return;

        IsClaimed = true;
    }
}

[Serializable]
public class Chapter
{
    public string chapterName;
    public List<ObjData> objectives = new();
}

public class JournalManager : MonoBehaviour
{
    public event Action<ObjData> OnObjectiveCompleted;

    [SerializeField] private GridManager gridManager;
    [SerializeField] private LaboratoryManager labManager;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private WaterManager waterManager;

    [SerializeField] List<Chapter> chaptersList;

    Dictionary<ObjData, ObjState> objectivesStates = new();
    Dictionary<ObjData, Chapter> objectiveToChapter = new();

    private void Awake()
    {
        foreach(Chapter chap in chaptersList)
        {
            foreach(ObjData obj in chap.objectives)
            {
                ObjState state = new ObjState();

                objectivesStates[obj] = state;
                objectiveToChapter[obj] = chap;
            }
        }
    }

    private void OnEnable()
    {
        resourceManager.OnCoinsEarned += HandleCoinsEarned;
    }

    private void OnDisable()
    {
        resourceManager.OnCoinsEarned -= HandleCoinsEarned;
    }

    public bool IsObjectiveComplete(ObjData obj) => objectivesStates[obj].IsComplete;
    public int GetProgress(ObjData obj) => objectivesStates[obj].Progress;

    private void HandleCoinsEarned(int amount)
    {
        foreach(ObjData obj in objectivesStates.Keys)
        {
            if(obj.Type == ObjectiveType.CoinsEarned )
            {
                ObjState state = objectivesStates[obj];

                if (state.IsComplete)
                    continue;

                state.IncreaseProgress(amount, obj.Target);

                if(state.IsComplete)
                    OnObjectiveCompleted?.Invoke(obj);
            }
        }
    }
}
