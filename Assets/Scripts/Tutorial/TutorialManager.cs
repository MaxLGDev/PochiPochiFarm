using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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
        Show(journalButton, "You will find what to do in the journal. \nTry clicking on it.", Step1JournalPresentation, 15f,
            pressTarget: true);
    }

    private void Step1JournalPresentation()
    {
        Show(journalPresentation, "Here is the full journal, with all chapters you will have to clear.", Step1Chapter, 25f);
    }

    private void Step1Chapter()
    {
        Show(chapter1Button, "Each chapter groups a set of goals. \nOpen Chapter 1.", Step1Objective, 10f, -20f,
            pressTarget: true);
    }

    private void Step1Objective()
    {
        Show(objective1Pos,
            "Those are the current chapter objectives. \nCome back here regularly to see if you completed one.",
            Step1Close, 20f, 10f);
    }

    private void Step1Close()
    {
        Show(journalCloseButton, "Close the journal, \nand let's take a look at the farm.", Step2Dirt, -10f, -20f,
            pressTarget: true);
    }
    
    // ------------------------------------------------------------------
    // STEP 2 - Farm Overview
    // ------------------------------------------------------------------

    private void Step2Dirt()
    {
        Show(farmBox, "This is your farm. Everything starts from the dirt, in the bottom left position. \nGather some to sell it later.", Step2Sell, -30f,
            pressTarget: true);
    }

    private void Step2Sell()
    {
        Show(sellButton, "Press sell to turn your harvest into golds.", Step2SellSpecificCrop, 10f, -15f);
    }

    private void Step2SellSpecificCrop()
    {
        Show(sellCropButton, "Or you can choose to sell \na specific crop by clicking on its box.", Step2Gold, 10f, -15f);
    }

    private void Step2Gold()
    {
        Show(goldsDisplay, "This is your current golds, \nwhich will be used for almost everything.", Step2Water, 10f,
            20f);
    }

    private void Step2Water()
    {
        Show(waterDisplay, "This is your current water, \nwhich you will need to grow crops.", Step2TileUnlock, -10f, -20f);
    }

    private void Step2TileUnlock()
    {
        Show(farmBox,
            "With golds, and the necessary chapters unlocked, \nyou will be able to unlock tiles by clicking on it. \nThen the farming can start!",
            Step3Lab, -29f);
    }
    
    // ------------------------------------------------------------------
    // STEP 3 - Laboratory & Upgrades
    // ------------------------------------------------------------------

    private void Step3Lab()
    {
        Show(labRect, "After claiming chapter 1, you will unlock the laboratory. \nIt allows you to research and automate crops. It's very important!", Step3Upgrade, -10f);
    }

    private void Step3Upgrade()
    {
        Show(upgradeRect,
            "You can also buy upgrades to make your life easier, check it out sometimes! \nThis also requires chapter 1 to be claimed.",
            Step3Final, -10f);
    }

    private void Step3Final()
    {
        Show(statsHeader, "This concludes the tutorial. \nGet all the upgrades, research and automate everything, " +
                          "\nand once the chapter 4 of the journal is claimed, \nyou will have finished the game! " +
                          "\nPochiPochi is watching you, so good luck!", EndTutorial, 20f, 45f);
    }

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    /// <summary>
    /// Highlights a target, shows text, and runs 'next' when the player clicks the holeFrame.
    /// If pressTarget is true, the target's real Button is also triggered
    /// (so clicking the highlighted journal button really opens the journal).
    /// </summary>
    private void Show(RectTransform target, string text, Action next, float textYOffset = 0f, float textXOffset = 0f, bool pressTarget = false)
    {
        holeButton.onClick.RemoveAllListeners();
        holeFrame.transform.SetAsLastSibling();
        typewriter.ShowText(text);
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
