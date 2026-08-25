using System;
using System.Collections.Generic;

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

        if (Progress >= target)
        {
            Progress = target;
            SetObjectiveComplete();
        }
    }

    public void SetObjectiveComplete() => IsComplete = true;
    public bool SetObjectiveClaimed()
    {
        if (!IsComplete)
            return false;

        IsClaimed = true;
        return true;
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
    public event Action OnChapter1Claimed;
    public event Action OnObjectiveClaimed;

    private Action<int> onCoinsEarnedHandler;
    private Action onTileHarvestedHandler;
    private Action<int> onWaterRefilledHandler;
    private Action onCropGatheredHandler;
    private Action<CropData> onRequestedCropUnlockedHandler;
    private Action<CropData> onRequestedCropResearchedHandler;
    private Action<CropData> onRequestedCropAutomatedHandler;


    public event Action<ObjData> OnObjectiveCompleted;
    public event Action<ObjData> OnObjectiveProgressed;

    [SerializeField] private GridManager gridManager;
    [SerializeField] private LaboratoryManager labManager;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private WaterManager waterManager;

    [SerializeField] List<Chapter> chaptersList;

    Dictionary<ObjData, ObjState> objectivesStates = new();
    Dictionary<ObjData, Chapter> objectiveToChapter = new();
    Dictionary<Chapter, bool> chapterUnlocked = new();

    private void Awake()
    {
        onCoinsEarnedHandler = (amount) => HandleObjectiveProgress(ObjectiveType.CoinsEarned, amount);
        onTileHarvestedHandler = () => HandleObjectiveProgress(ObjectiveType.ClickCount, 1);
        onWaterRefilledHandler = (amount) => HandleObjectiveProgress(ObjectiveType.WaterRefilled, amount);
        onCropGatheredHandler = () => HandleObjectiveProgress(ObjectiveType.CropGathered, 1);
        onRequestedCropUnlockedHandler = (crop) => HandleObjectiveProgress(ObjectiveType.UnlockCrop, 1, crop);
        onRequestedCropResearchedHandler = (crop) => HandleObjectiveProgress(ObjectiveType.ResearchCrop, 1, crop);
        onRequestedCropAutomatedHandler = (crop) => HandleObjectiveProgress(ObjectiveType.AutomateCrop, 1, crop);

        for (int i = 0; i < chaptersList.Count; i++)
        {
            Chapter chap = chaptersList[i];
            chapterUnlocked[chap] = (i == 0);

            foreach (ObjData obj in chap.objectives)
            {
                ObjState state = new ObjState();

                objectivesStates[obj] = state;
                objectiveToChapter[obj] = chap;
            }
        }
    }

    private void OnEnable()
    {
        resourceManager.OnCoinsEarned += onCoinsEarnedHandler;
        gridManager.OnTileHarvested += onTileHarvestedHandler;
        waterManager.OnWaterRefilled += onWaterRefilledHandler;
        gridManager.OnCropGathered += onCropGatheredHandler;
        gridManager.OnCropUnlocked += onRequestedCropUnlockedHandler;
        labManager.OnRequestedCropResearched += onRequestedCropResearchedHandler;
        labManager.OnRequestedCropAutomated += onRequestedCropAutomatedHandler;
    }

    private void OnDisable()
    {
        resourceManager.OnCoinsEarned -= onCoinsEarnedHandler;
        gridManager.OnTileHarvested -= onTileHarvestedHandler;
        waterManager.OnWaterRefilled -= onWaterRefilledHandler;
        gridManager.OnCropGathered -= onCropGatheredHandler;
        gridManager.OnCropUnlocked -= onRequestedCropUnlockedHandler;
        labManager.OnRequestedCropResearched -= onRequestedCropResearchedHandler;
        labManager.OnRequestedCropAutomated -= onRequestedCropAutomatedHandler;
    }

    public bool IsChapterUnlocked(Chapter chapter) => chapterUnlocked[chapter];
    public bool IsChapterFullyClaimed(Chapter chapter) => GetChapterProgress(chapter.objectives[0]).completed == chapter.objectives.Count;
    public void UnlockNextChapter(Chapter chapter)
    {
        int currentIndex = chaptersList.IndexOf(chapter);
        int nextIndex = currentIndex + 1;

        if (currentIndex < 0)
            return;

        if (nextIndex >= chaptersList.Count)
            return;

        chapterUnlocked[chaptersList[nextIndex]] = true;
        gridManager.UnlockZone(chapter);

        if (nextIndex == 1) // Assuming chapter 1 is at index 0
            OnChapter1Claimed?.Invoke();
    }

    public int GetProgress(ObjData obj) => objectivesStates[obj].Progress;
    public bool IsObjectiveComplete(ObjData obj) => objectivesStates[obj].IsComplete;
    public bool IsObjectiveClaimed(ObjData obj) => objectivesStates[obj].IsClaimed;

    public void ClaimObjective(ObjData obj)
    {
        if(objectivesStates[obj].SetObjectiveClaimed())
            OnObjectiveClaimed?.Invoke();
    }

    public (int completed, int total) GetChapterProgress(ObjData obj)
    {
        Chapter chapter = objectiveToChapter[obj];
        int completed = 0;
        int total = chapter.objectives.Count;
        foreach (ObjData chapterObj in chapter.objectives)
        {
            if (IsObjectiveClaimed(chapterObj))
                completed++;
        }
        return (completed, total);
    }

    private void HandleObjectiveProgress(ObjectiveType type, int amount, CropData requestedCrop = null)
    {
        foreach (ObjData obj in objectivesStates.Keys)
        {
            if (obj.Type == type)
            {
                ObjState state = objectivesStates[obj];

                if (state.IsComplete)
                    continue;

                if (obj.RelatedCrop != null && obj.RelatedCrop != requestedCrop)
                    continue;

                state.IncreaseProgress(amount, obj.Target);

                OnObjectiveProgressed?.Invoke(obj);

                if (state.IsComplete)
                    OnObjectiveCompleted?.Invoke(obj);
            }
        }
    }

    public Chapter GetChapter(int index) => chaptersList[index];

    public (int completed, int total) GetTotalJournalProgress()
    {
        int completed = 0;
        int total = 0;

        foreach(Chapter chap in chaptersList)
        {
            foreach(ObjData obj in chap.objectives)
            {
                total++;

                if (IsObjectiveClaimed(obj))
                    completed++;
            }
        }

        return (completed, total);
    }
}
