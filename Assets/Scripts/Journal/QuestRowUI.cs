using System;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestRowUI : MonoBehaviour
{
    public event Action OnQuestClaimed;

    private ObjData objectiveData;
    private JournalManager journalManager;

    [SerializeField] private TMP_Text questObjNameText;
    [SerializeField] private TMP_Text questObjGoalText;

    [SerializeField] private GameObject objectiveCompletePanel;
    [SerializeField] private GameObject objectiveClaimedPanel;
    [SerializeField] private Button objectiveCompleteButton;

    private void OnDisable()
    {
        if(journalManager != null)
        {
            journalManager.OnObjectiveProgressed -= HandleObjectiveProgressed;
            journalManager.OnObjectiveCompleted -= HandleObjectiveProgressed;
        }
    }

    public void Setup(ObjData obj, JournalManager manager)
    {
        objectiveData = obj;
        journalManager = manager;

        journalManager.OnObjectiveProgressed += HandleObjectiveProgressed;
        journalManager.OnObjectiveCompleted += HandleObjectiveProgressed;

        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        if (questObjNameText == null)
        {
            Debug.Log("NameText is missing");
            return;
        }


        if (questObjGoalText == null)
        {
            Debug.Log("GoalText is missing");
            return;
        }

        questObjNameText.text = objectiveData.Description;

        if (!journalManager.IsObjectiveComplete(objectiveData))
            questObjGoalText.text = $"<color=red>{journalManager.GetProgress(objectiveData)}/{objectiveData.Target}</color>";
        else
            questObjGoalText.text = $"<color=green>{journalManager.GetProgress(objectiveData)}/{objectiveData.Target}</color>";

        ToggleClaimedObjectivePanel();
        ToggleCompletedObjectivePanel();
    }

    private void ToggleCompletedObjectivePanel()
    {
        if (objectiveCompletePanel == null)
        {
            Debug.Log("ObjectiveCompletePanel is missing");
            return;
        }

        if (journalManager.IsObjectiveComplete(objectiveData) && !journalManager.IsObjectiveClaimed(objectiveData))
            objectiveCompletePanel.SetActive(true);
        else
            objectiveCompletePanel.SetActive(false);
    }

    private void ToggleClaimedObjectivePanel()
    {
        if (objectiveClaimedPanel == null)
        {
            Debug.Log("ObjectiveClaimedPanel is missing");
            return;
        }

        if (journalManager.IsObjectiveClaimed(objectiveData))
            objectiveClaimedPanel.SetActive(true);
        else
            objectiveClaimedPanel.SetActive(false);
    }

    public void HandleObjectiveProgressed(ObjData obj)
    {
        if (obj != this.objectiveData)
            return;

        if (questObjNameText == null)
        {
            Debug.Log("NameText is missing");
            return;
        }


        if (questObjGoalText == null)
        {
            Debug.Log("GoalText is missing");
            return;
        }

        questObjNameText.text = obj.Description;

        if (!journalManager.IsObjectiveComplete(obj))
            questObjGoalText.text = $"<color=red>{journalManager.GetProgress(obj)}/{obj.Target}</color>";
        else
            questObjGoalText.text = $"<color=green>{journalManager.GetProgress(obj)}/{obj.Target}</color>";

        RefreshDisplay();
    }

    public void MarkQuestAsClaimed()
    {
        if (objectiveClaimedPanel == null)
        {
            Debug.Log("ObjectiveClaimedPanel is missing");
            return;
        }

        if (objectiveCompletePanel.activeSelf && journalManager.IsObjectiveComplete(objectiveData))
        {
            journalManager.ClaimObjective(objectiveData);
            objectiveCompleteButton.interactable = false;
            objectiveCompletePanel.SetActive(false);
            objectiveClaimedPanel.SetActive(true);
            OnQuestClaimed?.Invoke();
        }

    }
}
