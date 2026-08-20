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
    [SerializeField] private float activeOffset = 15f;

    private Vector2[] originalTabPositions;

    private List<QuestRowUI> currentRows = new();
    private Chapter currentChapter;

    private void Awake()
    {
        originalTabPositions = new Vector2[tabs.Length];

        for (int i = 0; i < tabs.Length; i++)
            originalTabPositions[i] = tabs[i].anchoredPosition;
    }

    private void Start()
    {
        introChapter.SetActive(true);
        SetActiveTab(0);
        contentsChapters.SetActive(false);
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
        SetActiveTab(chapterIndex + 1);
        ShowChapter(chapter);
        UpdateClearedObjectivesText();
    }

    public void ToggleJournalPanel()
    {
        if (journalPanel != null)
            journalPanel.SetActive(!journalPanel.activeSelf);

        if (currentChapter != null)
            ShowChapter(currentChapter);
    }

    private void UpdateClearedObjectivesText()
    {
        if (currentChapter == null)
            return;

        int completed = 0;
        int total = currentChapter.objectives.Count;

        clearedObjectivesCounterText.text = $"<color=red>{completed}/{total}</color>";

        claimRewardsButton.interactable = false;
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
        {
            clearedObjectivesCounterText.text = $"<color=green>{completed}/{total}</color>";
            claimRewardsButton.interactable = true;
        }
        else
        {
            clearedObjectivesCounterText.text = $"<color=red>{completed}/{total}</color>";
            claimRewardsButton.interactable = false;
        }
    }


}
