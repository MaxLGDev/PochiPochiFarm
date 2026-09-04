using System;
using System.Collections.Generic;
using UnityEngine;


// ============================================
// Enums
// ============================================

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


// ============================================
// Objective State
// ============================================

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

    public void SetObjectiveComplete()
    {
        IsComplete = true;
    }

    public bool SetObjectiveClaimed()
    {
        if (!IsComplete)
            return false;

        IsClaimed = true;
        return true;
    }
}


// ============================================
// Chapter Data
// ============================================

[Serializable]
public class Chapter
{
    public string chapterName;
    public List<ObjData> objectives = new();
}


// ============================================
// Journal Manager
// ============================================

public class JournalManager : MonoBehaviour
{
    // --- Events ---
    public event Action OnChapter1Claimed;
    public event Action OnLastChapterClaimed;
    public event Action OnObjectiveClaimed;
    public event Action<ObjData> OnObjectiveCompleted;
    public event Action<ObjData> OnObjectiveProgressed;

    // --- Event Handlers ---
    private Action<int> onCoinsEarnedHandler;
    private Action onTileHarvestedHandler;
    private Action<int> onWaterRefilledHandler;
    private Action onCropGatheredHandler;
    private Action<CropData> onRequestedCropUnlockedHandler;
    private Action<CropData> onRequestedCropResearchedHandler;
    private Action<CropData> onRequestedCropAutomatedHandler;

    // --- References ---
    [SerializeField] private GridManager gridManager;
    [SerializeField] private LaboratoryManager labManager;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private WaterManager waterManager;

    // --- Chapter Data ---
    [SerializeField] private List<Chapter> chaptersList;

    // --- Runtime State ---
    private Dictionary<ObjData, ObjState> objectivesStates = new();
    private Dictionary<ObjData, Chapter> objectiveToChapter = new();
    private Dictionary<Chapter, bool> chapterUnlocked = new();


    // ==============================
    // Unity Lifecycle
    // ==============================

    private void Awake()
    {
        // Create event handlers so they can be subscribed and unsubscribed reliably.
        onCoinsEarnedHandler =
            amount => HandleObjectiveProgress(ObjectiveType.CoinsEarned, amount);

        onTileHarvestedHandler =
            () => HandleObjectiveProgress(ObjectiveType.ClickCount, 1);

        onWaterRefilledHandler =
            amount => HandleObjectiveProgress(ObjectiveType.WaterRefilled, amount);

        onCropGatheredHandler =
            () => HandleObjectiveProgress(ObjectiveType.CropGathered, 1);

        onRequestedCropUnlockedHandler =
            crop => HandleObjectiveProgress(ObjectiveType.UnlockCrop, 1, crop);

        onRequestedCropResearchedHandler =
            crop => HandleObjectiveProgress(ObjectiveType.ResearchCrop, 1, crop);

        onRequestedCropAutomatedHandler =
            crop => HandleObjectiveProgress(ObjectiveType.AutomateCrop, 1, crop);

        // Initialize the runtime state for every chapter and objective.
        for (int i = 0; i < chaptersList.Count; i++)
        {
            Chapter chapter = chaptersList[i];
            chapterUnlocked[chapter] = i == 0;

            foreach (ObjData objective in chapter.objectives)
            {
                ObjState state = new ObjState();

                objectivesStates[objective] = state;
                objectiveToChapter[objective] = chapter;
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


    // ==============================
    // Chapter
    // ==============================

    public bool IsChapterUnlocked(Chapter chapter)
    {
        return chapterUnlocked[chapter];
    }

    public bool IsChapterFullyClaimed(Chapter chapter)
    {
        return GetChapterProgress(chapter.objectives[0]).completed ==
               chapter.objectives.Count;
    }

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

        switch (nextIndex)
        {
            // Chapter 1 is assumed to be at index 0.
            case 1:
                OnChapter1Claimed?.Invoke();
                break;

            case 4:
                OnLastChapterClaimed?.Invoke();
                break;
        }
    }

    public Chapter GetChapter(int index)
    {
        return chaptersList[index];
    }

    public Chapter GetChapterForObjective(ObjData obj)
    {
        return objectiveToChapter[obj];
    }

    public Chapter GetNextChapter(Chapter chapter)
    {
        int index = chaptersList.IndexOf(chapter);
        int nextIndex = index + 1;

        return nextIndex < chaptersList.Count
            ? chaptersList[nextIndex]
            : null;
    }


    // ==============================
    // Objectives
    // ==============================

    public int GetProgress(ObjData obj)
    {
        return objectivesStates[obj].Progress;
    }

    public bool IsObjectiveComplete(ObjData obj)
    {
        return objectivesStates[obj].IsComplete;
    }

    public bool IsObjectiveClaimed(ObjData obj)
    {
        return objectivesStates[obj].IsClaimed;
    }

    public void ClaimObjective(ObjData obj)
    {
        if (objectivesStates[obj].SetObjectiveClaimed())
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

    public (int completed, int total) GetTotalJournalProgress()
    {
        int completed = 0;
        int total = 0;

        foreach (Chapter chapter in chaptersList)
        {
            foreach (ObjData objective in chapter.objectives)
            {
                total++;

                if (IsObjectiveClaimed(objective))
                    completed++;
            }
        }

        return (completed, total);
    }


    // ==============================
    // Event Handlers
    // ==============================

    private void HandleObjectiveProgress(
        ObjectiveType type,
        int amount,
        CropData requestedCrop = null)
    {
        foreach (ObjData objective in objectivesStates.Keys)
        {
            if (objective.Type != type)
                continue;

            ObjState state = objectivesStates[objective];
            Chapter chapter = objectiveToChapter[objective];

            if (!chapterUnlocked[chapter])
                continue;

            if (state.IsComplete)
                continue;

            // Crop-specific objectives only respond to their related crop.
            if (objective.RelatedCrop != null &&
                objective.RelatedCrop != requestedCrop)
            {
                continue;
            }

            state.IncreaseProgress(amount, objective.Target);

            OnObjectiveProgressed?.Invoke(objective);

            if (state.IsComplete)
                OnObjectiveCompleted?.Invoke(objective);
        }
    }
}