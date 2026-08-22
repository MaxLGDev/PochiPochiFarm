using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JournalUI : MonoBehaviour
{
    [SerializeField] private JournalManager journalManager;
    [SerializeField] private GameObject journalPanel;
    [SerializeField] private GameObject introChapter;
    [SerializeField] private GameObject contentsChapters;

    [SerializeField] private TMP_Text clearedObjectivesCounterText;
    [SerializeField] private Button claimRewardsButton;

    [SerializeField] private GameObject rowPrefab;
    [SerializeField] private Transform rowParent;

    [SerializeField] private RectTransform[] tabs;
    [SerializeField] private Button[] tabButtons;
    [SerializeField] private float activeOffset = 15f;

    [SerializeField] private GameObject pageFlipObject;
    [SerializeField] private Animator pageFlipAnimator;
    [SerializeField] private AnimationClip pageFlip;
    [SerializeField] private float pageFlipMultiplier = 1.3f;
    private int currentChapterIndex = -1;
    private float pageFlipDuration;

    private Vector2[] originalTabPositions;

    private List<QuestRowUI> currentRows = new();
    private Chapter currentChapter;

    private void Awake()
    {
        originalTabPositions = new Vector2[tabs.Length];

        for (int i = 0; i < tabs.Length; i++)
            originalTabPositions[i] = tabs[i].anchoredPosition;

        pageFlipDuration = pageFlip.length;
    }

    private void Start()
    {
        introChapter.SetActive(true);
        SetActiveTab(0);
        contentsChapters.SetActive(false);
        RefreshTabLocks();
    }

    private void OnEnable()
    {
        journalManager.OnObjectiveCompleted += HandleObjectiveCompleted;
    }

    private void OnDisable()
    {
        journalManager.OnObjectiveCompleted -= HandleObjectiveCompleted;
    }

    public void SetActiveTab(int index)
    {
        for(int i = 0; i < tabs.Length; i++)
        {
            tabs[i].anchoredPosition = originalTabPositions[i];

            if (i == index)
                tabs[i].anchoredPosition += Vector2.right * activeOffset;
        }
    }

    public void ShowIntro()
    {
        if (introChapter == null || contentsChapters == null)
            return;

        if (introChapter.activeSelf)
            return;

        if (currentChapterIndex != -1)
            PlayPageFlip(0, currentChapterIndex + 1);

        currentChapterIndex = 0;
        SetActiveTab(0);
        contentsChapters.SetActive(false);
        introChapter.SetActive(true);
    }

    public void ShowContents()
    {
        if (contentsChapters == null || introChapter == null)
            return;

        if (contentsChapters.activeSelf)
            return;

        introChapter.SetActive(false);
        contentsChapters.SetActive(true);
    }

    public void ShowChapter(Chapter chapter)
    {
        Debug.Log($"SHOWING CHAPTER: {chapter.chapterName}");
        foreach (var row in currentRows)
            Destroy(row.gameObject);

        currentRows.Clear();

        foreach (var obj in chapter.objectives)
        {
            Debug.Log($"CREATING ROW FOR: {obj.Description}");
            var row = Instantiate(rowPrefab, rowParent).GetComponent<QuestRowUI>();
            row.Setup(obj, journalManager);
            row.OnQuestClaimed += () => HandleObjectiveCompleted(obj);
            currentRows.Add(row);
        }
    }

    public void SelectChapter(Chapter chapter, int chapterIndex)
    {
        currentChapter = chapter;

        if(currentChapterIndex != -1)
            PlayPageFlip(chapterIndex + 1, currentChapterIndex + 1);

        currentChapterIndex = chapterIndex;
        SetActiveTab(chapterIndex + 1);
        ShowChapter(chapter);
        HandleObjectiveCompleted(chapter.objectives[0]);
    }

    public void ClaimChapterRewards()
    {
        journalManager.UnlockNextChapter(currentChapter);
        RefreshTabLocks();
    }

    public void RefreshTabLocks()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            Chapter chapter = journalManager.GetChapter(i);
            tabButtons[i].interactable = journalManager.IsChapterUnlocked(chapter);
        }
    }

    public void ToggleJournalPanel()
    {
        if (journalPanel != null)
        {
            bool opening = !journalPanel.activeSelf;
            journalPanel.SetActive(opening);

            if(opening)
            {
                currentChapterIndex = -1;
                PlayPageFlip(0, 0);
                currentChapterIndex = 0;

                SetActiveTab(0);
                contentsChapters.SetActive(false);
                introChapter.SetActive(false);
                ShowIntro();
            }
        }

        if (currentChapter != null)
            ShowChapter(currentChapter);
    }

    public void HandleObjectiveCompleted(ObjData obj)
    {
        var (completed, total) = journalManager.GetChapterProgress(obj);

        if (clearedObjectivesCounterText == null)
        {
            Debug.Log("ClearedObjectivesText is missing");
            return;
        }

        if (completed >= total)
            clearedObjectivesCounterText.text = $"<color=green>{completed}/{total}</color>";
        else
            clearedObjectivesCounterText.text = $"<color=red>{completed}/{total}</color>";

        claimRewardsButton.interactable = completed >= total;
    }

    private void PlayPageFlip(int newChapterIndex, int oldChapterIndex)
    {
        bool forward = newChapterIndex >= oldChapterIndex;
        string clipName = forward ? "PageFlip" : "PageFlipReverse";

        pageFlipObject.SetActive(true);
        pageFlipAnimator.Play(clipName, 0, 0f);

        StopAllCoroutines();
        StartCoroutine(HidePageFlipAfter(pageFlipDuration / pageFlipMultiplier));
    }

    private IEnumerator HidePageFlipAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        pageFlipObject.SetActive(false);
    }
}
