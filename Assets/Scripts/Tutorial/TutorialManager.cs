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
    [SerializeField] private RectTransform goldsDisplay;
    [SerializeField] private RectTransform waterDisplay;

    [Header("Step 3 - Laboratory and Upgrades")] 
    [SerializeField] private RectTransform labRect;
    [SerializeField] private RectTransform upgradeRect;
    [SerializeField] private RectTransform statsHeader;
    
    private bool hasDoneTutorial;

    private void Awake()
    {
        holeFrame.SetActive(false);
    }

    private void Start()
    {
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
    }
    
    // ------------------------------------------------------------------
    // STEP 1 - Showing the objectives
    // ------------------------------------------------------------------

    private void Step1Journal()
    {
        Show(journalButton, "You will find what to do in the journal. Try clicking on it.", Step1JournalPresentation,
            pressTarget: true);
    }

    private void Step1JournalPresentation()
    {
        Show(journalPresentation, "Here is the full journal, with all chapters you will have to clear.", Step1Chapter);
    }

    private void Step1Chapter()
    {
        Show(chapter1Button, "Each chapter groups a set of goals. Open Chapter 1.", Step1Objective,
            pressTarget: true);
    }

    private void Step1Objective()
    {
        Show(objective1Pos, "Those are the current chapter objectives. Come back here regularly to see if you completed one.",
            Step1Close);
    }

    private void Step1Close()
    {
        Show(journalCloseButton, "Close the journal and let's take a look at the farm.", Step2Dirt,
            pressTarget: true);
    }
    
    // ------------------------------------------------------------------
    // STEP 2 - Farm Overview
    // ------------------------------------------------------------------

    private void Step2Dirt()
    {
        Show(farmBox, "This is your farm. Everything starts from the dirt, in the bottom left position. Gather some and let's sell it.", Step2Sell,
            pressTarget: true);
    }

    private void Step2Sell()
    {
        Show(sellButton, "Press sell to turn your harvest into golds.", Step2Gold);
    }

    private void Step2Gold()
    {
        Show(goldsDisplay, "This is your current golds, which will be used for almost everything.", Step2Water);
    }

    private void Step2Water()
    {
        Show(waterDisplay, "This is your current water, which you will need to grow crops.", Step2TileUnlock);
    }

    private void Step2TileUnlock()
    {
        Show(farmBox,
            "With golds, and the necessary chapters unlocked, you will be able to unlock tiles by clicking on it. Then the farming can start!",
            Step3Lab);
    }
    
    // ------------------------------------------------------------------
    // STEP 3 - Laboratory & Upgrades
    // ------------------------------------------------------------------

    private void Step3Lab()
    {
        Show(labRect, "After claiming chapter 1, you will unlock the laboratory. It allows you to research and automate crops. It's very important!", Step3Upgrade);
    }

    private void Step3Upgrade()
    {
        Show(upgradeRect,
            "You can also buy upgrades to make your life easier, check it out sometimes! This also requires chapter 1 to be claimed.",
            Step3Final);
    }

    private void Step3Final()
    {
        Show(statsHeader, "This concludes the tutorial. Get all the upgrades, research and automate everything, " +
                          "and once the chapter 4 of the journal is claimed, you will have finished the game! " +
                          "PochiPochi is watching you, so good luck!", EndTutorial);
    }

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    /// <summary>
    /// Highlights a target, shows text, and runs 'next' when the player clicks the holeFrame.
    /// If pressTarget is true, the target's real Button is also triggered
    /// (so clicking the highlighted journal button really opens the journal).
    /// </summary>
    private void Show(RectTransform target, string text, Action next, bool pressTarget = false)
    {
        Debug.Log($"[Tuto] Show -> target={target.name}, next={next.Method.Name}");

        holeButton.onClick.RemoveAllListeners();
        holeFrame.transform.SetAsLastSibling();
        holeText.text = text;
        overlay.Highlight(target);

        holeButton.onClick.AddListener(() =>
        {
            Debug.Log($"[Tuto] holeButton CLICKED (target={target.name}, pressTarget={pressTarget})");

            if (pressTarget && target.TryGetComponent(out Button realButton))
            {
                Debug.Log("[Tuto] invoking real button");
                realButton.onClick.Invoke();
            }

            Debug.Log($"[Tuto] calling next: {next.Method.Name}");
            next?.Invoke();
        });
    }
}
