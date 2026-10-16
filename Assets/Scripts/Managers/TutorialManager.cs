using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Localization.Settings;

/// <summary>
/// Simple scripted tutorial: one method per highlight, chained one after another.
/// The player can only click the holeFrame, which sits on top of the highlighted element.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TutorialOverlay overlay;
    [SerializeField] private Typewriter typewriter;
    
    [Header("Tutorial UI")] 
    [SerializeField] private GameObject holeFrame;
    [SerializeField] private Button holeButton;
    [SerializeField] private TMP_Text holeText;

    [Header("Step 1 - Objectives")] 
    [SerializeField] private RectTransform journalButton;
    [SerializeField] private RectTransform journalPresentation;
    [SerializeField] private RectTransform chapter1Button;
    [SerializeField] private RectTransform objective1Pos;
    [SerializeField] private RectTransform journalCloseButton;

    [Header("Step 2 - Farm Overview")] 
    [SerializeField] private RectTransform farmBox;
    [SerializeField] private RectTransform sellButton;
    [SerializeField] private RectTransform sellCropButton;
    [SerializeField] private RectTransform refillWaterButton;
    [SerializeField] private RectTransform goldsDisplay;
    [SerializeField] private RectTransform waterDisplay;

    [Header("Step 3 - Laboratory and Upgrades")] 
    [SerializeField] private RectTransform labRect;
    [SerializeField] private RectTransform upgradeRect;
    [SerializeField] private RectTransform statsHeader;
    
    private bool tutorialCompleted;

    private void Awake()
    {
        holeFrame.SetActive(false);
    }

    private void Start()
    {
        if(!tutorialCompleted)
            StartTutorial();
    }

    public void StartTutorial()
    {
        holeFrame.SetActive(true);
        Step1Journal();
    }

    private void EndTutorial()
    {
        holeButton.onClick.RemoveAllListeners();
        holeFrame.SetActive(false);
        tutorialCompleted = true;
    }
    
    // ------------------------------------------------------------------
    // STEP 1 - Showing the objectives
    // ------------------------------------------------------------------

    private void Step1Journal()
    {
        Show(journalButton, "Tutorial.Step1Journal", Step1JournalPresentation, 15f,
            pressTarget: true);
    }

    private void Step1JournalPresentation()
    {
        Show(journalPresentation, "Tutorial.Step1JournalPresentation", Step1Chapter, 25f);
    }

    private void Step1Chapter()
    {
        Show(chapter1Button, "Tutorial.Step1Chapter", Step1Objective, 10f, -20f,
            pressTarget: true);
    }

    private void Step1Objective()
    {
        Show(objective1Pos,
            "Tutorial.Step1Objective",
            Step1Close, 20f, 10f);
    }

    private void Step1Close()
    {
        Show(journalCloseButton, "Tutorial.Step1Close", Step2Dirt, -10f, -20f,
            pressTarget: true);
    }
    
    // ------------------------------------------------------------------
    // STEP 2 - Farm Overview
    // ------------------------------------------------------------------

    private void Step2Dirt()
    {
        Show(farmBox, "Tutorial.Step2Dirt", Step2Sell, -30f,
            pressTarget: true);
    }

    private void Step2Sell()
    {
        Show(sellButton, "Tutorial.Step2Sell", Step2SellSpecificCrop, 10f, -15f);
    }

    private void Step2SellSpecificCrop()
    {
        Show(sellCropButton, "Tutorial.Step2SellSpecificCrop", Step2RefillWater, 10f, -15f);
    }

    private void Step2RefillWater()
    {
        Show(refillWaterButton, "Tutorial.Step2RefillWater", Step2Gold, -10f, -20f);
    }

    private void Step2Gold()
    {
        Show(goldsDisplay, "Tutorial.Step2Gold", Step2Water, 10f,
            20f);
    }

    private void Step2Water()
    {
        Show(waterDisplay, "Tutorial.Step2Water", Step2TileUnlock, -10f, 20f);
    }

    private void Step2TileUnlock()
    {
        Show(farmBox,
            "Tutorial.Step2TileUnlock",
            Step3Lab, -29f);
    }
    
    // ------------------------------------------------------------------
    // STEP 3 - Laboratory & Upgrades
    // ------------------------------------------------------------------

    private void Step3Lab()
    {
        Show(labRect, "Tutorial.Step3Lab", Step3Upgrade, -10f);
    }

    private void Step3Upgrade()
    {
        Show(upgradeRect,
            "Tutorial.Step3Upgrade",
            Step3Final, -10f);
    }

    private void Step3Final()
    {
        Show(statsHeader, "Tutorial.Step3Final", EndTutorial, 20f, 45f);
    }

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    /// <summary>
    /// Highlights a target, shows text, and runs 'next' when the player clicks the holeFrame.
    /// If pressTarget is true, the target's real Button is also triggered
    /// (so clicking the highlighted journal button really opens the journal).
    /// </summary>
    private void Show(RectTransform target, string key, Action next, float textYOffset = 0f, float textXOffset = 0f, bool pressTarget = false)
    {
        holeButton.onClick.RemoveAllListeners();
        holeFrame.transform.SetAsLastSibling();

        var tutorialLocalizedText = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Tutorial", key);
        tutorialLocalizedText.Completed += handle => typewriter.ShowText(handle.Result);
        
        overlay.Highlight(target);

        holeText.transform.position = holeButton.transform.position + new Vector3(textXOffset, textYOffset, 0f);

        holeButton.onClick.AddListener(() =>
        {
            if (pressTarget && target.TryGetComponent(out Button realButton))
                realButton.onClick.Invoke();
            
            next?.Invoke();
        });
    }

    public TutorialSaveData CaptureState()
    {
        TutorialSaveData data = new TutorialSaveData();
        data.hasDoneTutorial = tutorialCompleted;
        return data;
    }

    public void ApplyState(TutorialSaveData data)
    {
        if (data == null)
            return;
        
        tutorialCompleted = data.hasDoneTutorial;
    }
}
