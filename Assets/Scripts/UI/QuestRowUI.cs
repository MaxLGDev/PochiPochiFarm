using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class QuestRowUI : MonoBehaviour
{
    // --- Events ---
    public event Action OnQuestClaimed;

    // --- State ---
    private ObjData objectiveData;
    private JournalManager journalManager;
    

    // --- UI References ---
    [SerializeField] private TMP_Text questObjGoalText;
    [SerializeField] private GameObject objectiveCompletePanel;
    [SerializeField] private GameObject objectiveClaimedPanel;
    [SerializeField] private Button objectiveCompleteButton;
    [SerializeField] private LocalizeStringEvent nameLocalizer;


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

        string cropName = objectiveData.RelatedCrop != null ? objectiveData.RelatedCrop.LocalizedName.GetLocalizedString() : string.Empty;

        var source = objectiveData.localizedDescription;
        var description = new LocalizedString(source.TableReference, source.TableEntryReference)
        {
            Arguments = new object[]
            {
            StatsFormatter.FormatNumber(objectiveData.Target),
            cropName
            }
        };
        nameLocalizer.StringReference = description;

        RefreshDisplay();
    }


    // ==============================
    // Display
    // ==============================

    public void RefreshDisplay()
    {
        if (questObjGoalText == null)
        {
            Debug.Log("GoalText is missing");
            return;
        }

        int progress = journalManager.GetProgress(objectiveData);
        int target = objectiveData.Target;
        bool isComplete = journalManager.IsObjectiveComplete(objectiveData);

        string color = isComplete ? "green" : "red";
        questObjGoalText.text =
            $"<color={color}>{StatsFormatter.FormatNumber(progress)}/{StatsFormatter.FormatNumber(target)}</color>";

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
            SoundManager.Instance.PlaySFX("ObjectiveClaimed");
            
            objectiveCompleteButton.interactable = false;
            objectiveCompletePanel.SetActive(false);
            objectiveClaimedPanel.SetActive(true);

            OnQuestClaimed?.Invoke();
        }
    }
}