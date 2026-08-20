using TMPro;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class JournalUI : MonoBehaviour
{
    [SerializeField] private JournalManager journalManager;
    [SerializeField] private GameObject journalPanel;

    [SerializeField] private TMP_Text clearedObjectivesCounterText;
    [SerializeField] private Button claimRewardsButton;

    [SerializeField] private GameObject rowPrefab;
    [SerializeField] private Transform rowParent;

    private List<QuestRowUI> currentRows = new();
    private Chapter currentChapter;

    private void OnEnable()
    {
        journalManager.OnObjectiveCompleted += HandleObjectiveCompleted;
    }

    private void OnDisable()
    {
        journalManager.OnObjectiveCompleted -= HandleObjectiveCompleted;
    }

    public void ShowChapter(Chapter chapter)
    {
        foreach(var row in currentRows)
            Destroy(row.gameObject);

        currentRows.Clear();

        foreach(var obj in chapter.objectives)
        {
            var row = Instantiate(rowPrefab, rowParent).GetComponent<QuestRowUI>();
            row.Setup(obj, journalManager);
            row.OnQuestClaimed += () => HandleObjectiveCompleted(obj);
            currentRows.Add(row);
        }
    }

    public void SelectChapter(Chapter chapter)
    {
        currentChapter = chapter;
        ShowChapter(chapter);
    }

    public void ToggleJournalPanel()
    {
        if (journalPanel != null)
            journalPanel.SetActive(!journalPanel.activeSelf);

        if(currentChapter != null)
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

        if(completed >= total)
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
