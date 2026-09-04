using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestRowUI : MonoBehaviour
{
    // --- Events ---
    public event Action OnQuestClaimed;

    // --- State ---
    private ObjData objectiveData;
    private JournalManager journalManager;

    // --- UI References ---
    [SerializeField] private TMP_Text questObjNameText;
    [SerializeField] private TMP_Text questObjGoalText;
    [SerializeField] private GameObject objectiveCompletePanel;
    [SerializeField] private GameObject objectiveClaimedPanel;
    [SerializeField] private Button objectiveCompleteButton;


    // ==============================
    // Unity Lifecycle
    // ==============================

    private void OnDisable()
    {
        if (journalManager == null)
            return;

        journalManager.OnObjectiveProgressed -= HandleObjectiveProgressed;
        journalManager.OnObjectiveCompleted -= HandleObjectiveProgressed;
    }


    // ==============================
    // Setup
    // ==============================

    public void Setup(ObjData obj, JournalManager manager)
    {
        objectiveData = obj;
        journalManager = manager;

        journalManager.OnObjectiveProgressed += HandleObjectiveProgressed;
        journalManager.OnObjectiveCompleted += HandleObjectiveProgressed;

        RefreshDisplay();
    }


    // ==============================
    // Display
    // ==============================

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

        int progress = journalManager.GetProgress(objectiveData);
        int target = objectiveData.Target;
        bool isComplete = journalManager.IsObjectiveComplete(objectiveData);

        string color = isComplete ? "green" : "red";
        questObjGoalText.text =
            $"<color={color}>{FormatNumber(progress)}/{FormatNumber(target)}</color>";

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

        bool shouldShow =
            journalManager.IsObjectiveComplete(objectiveData) &&
            !journalManager.IsObjectiveClaimed(objectiveData);

        objectiveCompletePanel.SetActive(shouldShow);
    }

    private void ToggleClaimedObjectivePanel()
    {
        if (objectiveClaimedPanel == null)
        {
            Debug.Log("ObjectiveClaimedPanel is missing");
            return;
        }

        objectiveClaimedPanel.SetActive(
            journalManager.IsObjectiveClaimed(objectiveData)
        );
    }


    // ==============================
    // Event Handlers
    // ==============================

    public void HandleObjectiveProgressed(ObjData obj)
    {
        if (obj != objectiveData)
            return;

        RefreshDisplay();
    }


    // ==============================
    // Quest Actions
    // ==============================

    public void MarkQuestAsClaimed()
    {
        if (objectiveClaimedPanel == null)
        {
            Debug.Log("ObjectiveClaimedPanel is missing");
            return;
        }

        if (objectiveCompletePanel.activeSelf &&
            journalManager.IsObjectiveComplete(objectiveData))
        {
            journalManager.ClaimObjective(objectiveData);

            objectiveCompleteButton.interactable = false;
            objectiveCompletePanel.SetActive(false);
            objectiveClaimedPanel.SetActive(true);

            OnQuestClaimed?.Invoke();
        }
    }


    // ==============================
    // Formatting
    // ==============================

    private string FormatNumber(int value)
    {
        if (value >= 1_000_000)
            return $"{value / 1_000_000f:0.#}M";

        if (value >= 1_000)
            return $"{value / 1_000f:0.#}K";

        return value.ToString();
    }
}