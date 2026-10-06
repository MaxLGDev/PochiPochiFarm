using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Shows a full-screen popup when the laboratory finishes a crop
/// ("RESEARCHED!" / "AUTOMATED!") and when Chapter 1 is claimed
/// (Laboratory and Upgrades unlocked).
/// Only listens and displays, owns no game state.
/// Popups are queued so two requests at once don't overwrite each other.
/// </summary>
public class LabRequestCompleteUI : MonoBehaviour
{
    private Action onChapter1ClaimedHandler;
    
    // --- State ---
    private struct PopupRequest
    {
        public Sprite sprite; // Optional: null means no image is shown
        public string title;
        public string tip;
    }

    private readonly Queue<PopupRequest> requests = new();
    private bool isShowing;

    // --- Event Handlers ---
    private Action<CropData> onResearchedHandler;
    private Action<CropData> onAutomatedHandler;

    // --- References ---
    [SerializeField] private LaboratoryManager labManager;
    [SerializeField] private JournalManager journalManager;

    // The panel to show/hide. Must be a child of the object this script is on,
    // otherwise deactivating it would also disable the event subscriptions.
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TMP_Text cropTitleText;
    [SerializeField] private Image cropImage;
    [SerializeField] private TMP_Text closePanelText;
    [SerializeField] private TMP_Text tipText;

    // Image shown when chapter 1 is complete
    [SerializeField] private Sprite tabsUnlockedSprite;

    [SerializeField] private FadeAnim panelFade;
    [SerializeField] private float lockDuration = 3f;
    [SerializeField] private float fadeOutWait = 0.15f;

    private bool canClose;

    private void Awake()
    {
        onResearchedHandler = crop => Enqueue(crop.GrowthSprites[^1], "RESEARCHED!!", $"Tip: You can now manually gather {crop.CropName} by clicking on it!");
        onAutomatedHandler = crop => Enqueue(crop.GrowthSprites[^1], "AUTOMATED!!", $"Tip: {crop.CropName} will now harvest itself as long as you have enough water!");
        onChapter1ClaimedHandler = HandleChapter1Claimed;
    }

    private void Start()
    {
        popupPanel.SetActive(false);
    }

    private void OnEnable()
    {
        labManager.OnRequestedCropResearched += onResearchedHandler;
        labManager.OnRequestedCropAutomated += onAutomatedHandler;
        journalManager.OnChapter1Claimed += onChapter1ClaimedHandler;
    }

    private void OnDisable()
    {
        labManager.OnRequestedCropResearched -= onResearchedHandler;
        labManager.OnRequestedCropAutomated -= onAutomatedHandler;
        journalManager.OnChapter1Claimed -= onChapter1ClaimedHandler;
    }

    private void HandleChapter1Claimed()
    {
        Enqueue(tabsUnlockedSprite, "NEW TABS!!", "Laboratory and Upgrades tabs have been unlocked!");   
    }
    
    private void Enqueue(Sprite sprite, string title, string tip)
    {
        requests.Enqueue(new PopupRequest { sprite = sprite, title = title, tip = tip});

        // If a popup is already open, this request waits its turn
        if (!isShowing)
            ShowNext();
    }

    private void ShowNext()
    {
        if (requests.Count == 0)
        {
            isShowing = false;
            return;
        }

        isShowing = true;
        StartCoroutine(ShowRoutine(requests.Dequeue()));
    }

    private IEnumerator ShowRoutine(PopupRequest request)
    {
        canClose = false;

        cropImage.gameObject.SetActive(request.sprite != null);
        cropImage.sprite = request.sprite;
        
        cropTitleText.text = request.title;
        tipText.text = request.tip;
        closePanelText.gameObject.SetActive(false);

        // Start invisible, then activate and fade in.
        panelFade.SetInstant(false);
        popupPanel.SetActive(true);
        panelFade.Fade(true);
        tipText.gameObject.SetActive(true);

        yield return new WaitForSeconds(lockDuration);

        canClose = true;
        closePanelText.gameObject.SetActive(true);
    }

    public void Close()
    {
        if (!canClose)
            return;

        canClose = false;
        StartCoroutine(CloseRoutine());
    }

    private IEnumerator CloseRoutine()
    {
        panelFade.Fade(false);
        yield return new WaitForSeconds(fadeOutWait);

        popupPanel.SetActive(false);
        ShowNext();
    }
    
    
}