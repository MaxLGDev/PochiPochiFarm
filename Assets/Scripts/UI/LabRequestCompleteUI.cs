using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Localization;

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
        public LocalizedString title;
        public LocalizedString tip;
        public CropData crop;
        public LocalizedSprite localizedSprite;
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

    [SerializeField] private LocalizedString researchedText;
    [SerializeField] private LocalizedString researchedTipText;
    [SerializeField] private LocalizedString automatedText;
    [SerializeField] private LocalizedString automatedTipText;
    [SerializeField] private LocalizedString newTabsText;
    [SerializeField] private LocalizedString newTabsTipText;

    // Image shown when chapter 1 is complete
    [SerializeField] private LocalizedSprite tabsUnlockedSprite;

    [SerializeField] private FadeAnim panelFade;
    [SerializeField] private float lockDuration = 3f;
    [SerializeField] private float fadeOutWait = 0.15f;

    private bool canClose;

    private void Awake()
    {
        onResearchedHandler = crop => Enqueue(crop.GrowthSprites[^1], researchedText, researchedTipText, crop);
        onAutomatedHandler = crop => Enqueue(crop.GrowthSprites[^1], automatedText, automatedTipText, crop);
        onChapter1ClaimedHandler = HandleChapter1Claimed;
    }

    private void Start()
    {
        popupPanel.SetActive(false);
    }

    private void OnEnable()
    {
        labManager.OnCropResearched += onResearchedHandler;
        labManager.OnCropAutomated += onAutomatedHandler;
        journalManager.OnChapter1Claimed += onChapter1ClaimedHandler;
    }

    private void OnDisable()
    {
        labManager.OnCropResearched -= onResearchedHandler;
        labManager.OnCropAutomated -= onAutomatedHandler;
        journalManager.OnChapter1Claimed -= onChapter1ClaimedHandler;
    }

    private void HandleChapter1Claimed()
    {
        Enqueue(null, newTabsText, newTabsTipText, null, tabsUnlockedSprite);   
    }
    
    private void Enqueue(Sprite sprite, LocalizedString title, LocalizedString tip, CropData crop = null, LocalizedSprite localizedSprite = null)
    {
        requests.Enqueue(new PopupRequest { sprite = sprite, title = title, tip = tip, crop = crop, localizedSprite = localizedSprite});

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

        Sprite spriteToShow = request.sprite;
        if (request.localizedSprite != null && !request.localizedSprite.IsEmpty)
        {
            bool loaded = false;
            request.localizedSprite.LoadAssetAsync().Completed += handle => { spriteToShow = handle.Result; loaded = true; };
            yield return new WaitUntil(() => loaded);
        }
        
        cropImage.gameObject.SetActive(spriteToShow != null);
        Debug.Log(spriteToShow);
        cropImage.sprite = spriteToShow;

        if (request.crop != null)
        {
            string cropName = null;
            
            request.crop.LocalizedName.GetLocalizedStringAsync().Completed += handle => cropName = handle.Result;
            yield return new WaitUntil(() => cropName != null);
            
            request.title.Arguments = new object[] { cropName };
            request.tip.Arguments = new object[] { cropName };
        }

        string resolvedTitle = null;
        string resolvedTip = null;

        request.title.GetLocalizedStringAsync().Completed += handle => resolvedTitle = handle.Result;
        request.tip.GetLocalizedStringAsync().Completed += handle => resolvedTip = handle.Result;
        yield return new WaitUntil(() => resolvedTitle != null && resolvedTip != null);

        cropTitleText.text = resolvedTitle;
        tipText.text = resolvedTip;
        
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