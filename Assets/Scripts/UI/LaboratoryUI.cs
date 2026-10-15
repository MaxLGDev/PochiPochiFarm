using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

/// <summary>
/// Controls the Laboratory user interface.
///
/// Handles:
/// - Research UI
/// - Automation UI
/// - Crop selection
/// - Progress display
/// - Button availability
/// - UI animations when starting actions
/// </summary>
public class LaboratoryUI : MonoBehaviour
{
    /// <summary>
    /// Stores every UI reference required for a laboratory action
    /// (Research or Automation).
    ///
    /// Using a shared class prevents duplicated UI code.
    /// </summary>
    [Serializable]
    private class ActionUI
    {
        /// <summary>
        /// Crop currently selected in the dropdown.
        /// </summary>
        [NonSerialized] public CropData SelectedCrop;

        // Starts the action.
        public Button button;

        // Crop name.
        public TMP_Text buttonText;

        // Fade animation for the button.
        public FadeAnim buttonFade;

        // Fade animation for the progress bar.
        public FadeAnim sliderFade;

        // Progress bar.
        public Slider slider;

        // Progress percentage/status.
        public TMP_Text sliderText;

        public GameObject warningText;

        // Crop selection dropdown.
        public TMP_Dropdown dropdown;

        public LocalizedString actionLabel;

        public LocalizedString doneLabel;
        
        [NonSerialized] public string ActionText;
        [NonSerialized] public string DoneText;

        /// <summary>
        /// Reference to the currently running UI animation coroutine.
        /// Allows restarting without duplicates.
        /// </summary>
        [NonSerialized] public Coroutine ActiveCoroutine;

        [NonSerialized] public bool CompleteFadePlayed;

        public RainbowCyclingColor rainbowText;
    }

    [Serializable]
    public class CostSlotUI
    {
        public GameObject slotRoot;
        public Image slotIcon;
        public TMP_Text slotText;
    }

    //==========================================================================
    // References
    //==========================================================================

    [SerializeField] private LaboratoryManager labManager;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private JournalManager journalManager;

    [SerializeField] private List<CostSlotUI> researchCostSlots;
    [SerializeField] private List<CostSlotUI> automationCostSlots;

    [SerializeField] private Sprite doneMarkSprite;

    [SerializeField] private Button laboratoryButton;

    [SerializeField] private GameObject labPanel;

    // Coin icon used for both research and automation costs.
    [SerializeField] private Sprite coinIcon;

    // UI for the research panel.
    [SerializeField] private ActionUI researchUI;

    // UI for the automation panel.
    [SerializeField] private ActionUI automationUI;

    [SerializeField] private float fadeWait = 0.2f;

    private string[] cropNames;
    

    private bool laboratoryUnlocked;
    private bool researchLoopPlaying;
    private bool automationLoopPlaying;

    private readonly Dictionary<TMP_Dropdown, Sprite[]> originalIcons = new();

    private void Start()
    {
        CaptureOriginalIcons(researchUI.dropdown);
        CaptureOriginalIcons(automationUI.dropdown);
        
        labPanel.SetActive(false);
        SetLaboratoryUnlocked(journalManager.IsChapter1Claimed());

        researchUI.dropdown.onValueChanged.AddListener(OnResearchCropSelected);
        automationUI.dropdown.onValueChanged.AddListener(OnAutomationCropSelected);

        OnResearchCropSelected(researchUI.dropdown.value);
        OnAutomationCropSelected(automationUI.dropdown.value);
        
        RefreshCropNames();
        LoadButtonTexts(researchUI);
        LoadButtonTexts(automationUI);
    }

    private void HandleJournalChapter1Claimed()
    {
        SetLaboratoryUnlocked(true);
    }

    private void HandleResearchCompleted(CropData crop)
    {
        RefreshDropdownLabels(researchUI, labManager.IsCropResearched);
    }

    private void HandleAutomationCompleted(CropData crop)
    {
        RefreshDropdownLabels(automationUI, labManager.IsCropAutomated);
    }

    private void SetLaboratoryUnlocked(bool unlocked)
    {
        laboratoryUnlocked = unlocked;
        laboratoryButton.interactable = unlocked;
    }

    private void Update()
    {
        // Refresh both interfaces every frame.
        UpdateResearchUI();
        UpdateAutomationUI();
        RefreshProgressLoop();
    }

    private void OnEnable()
    {
        journalManager.OnChapter1Claimed += HandleJournalChapter1Claimed;
        labManager.OnCropResearched += HandleResearchCompleted;
        labManager.OnCropAutomated += HandleAutomationCompleted;
        LocalizationSettings.SelectedLocaleChanged += HandleLocaleChanged;
    }

    private void OnDisable()
    {
        journalManager.OnChapter1Claimed -= HandleJournalChapter1Claimed;
        labManager.OnCropResearched -= HandleResearchCompleted;
        labManager.OnCropAutomated -= HandleAutomationCompleted;
        LocalizationSettings.SelectedLocaleChanged -= HandleLocaleChanged;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopLoop("Research");
            SoundManager.Instance.StopLoop("Automation");
        }

        researchLoopPlaying = automationLoopPlaying = false;

    }

    private void RefreshDropdownLabels(ActionUI ui, Func<CropData, bool> isDone)
    {
        for (int i = 0; i < ui.dropdown.options.Count; i++)
        {
            CropData crop = labManager.GetCropAt(i);

            string newName = cropNames[i] ?? crop.CropName; // falls back to normal name if issue arises

            ui.dropdown.options[i].text = isDone(crop) ? $"<s>{newName}</s>" : newName;

            ui.dropdown.options[i].image = isDone(crop)
                ? doneMarkSprite
                : originalIcons[ui.dropdown][i];
        }
        
        ui.dropdown.RefreshShownValue();
    }

    private void CaptureOriginalIcons(TMP_Dropdown dropdown)
    {
        Sprite[] icons = new Sprite[dropdown.options.Count];

        for (int i = 0; i < icons.Length; i++)
            icons[i] = dropdown.options[i].image;

        originalIcons[dropdown] = icons;
    }

    private void RefreshCropNames()
    {
        int count = researchUI.dropdown.options.Count;
        cropNames = new string[count];
        int pending = count;
        
        Debug.Log($"research: {researchUI.dropdown.options.Count}, automation: {automationUI.dropdown.options.Count}");
        
        for(int i = 0; i < count; i++)
        {
            int index = i;
            labManager.GetCropAt(i).LocalizedName.GetLocalizedStringAsync().Completed += handle =>
            {
                cropNames[index] = handle.Result;
                pending--;

                if (pending == 0)
                {
                    RefreshDropdownLabels(researchUI, labManager.IsCropResearched);
                    RefreshDropdownLabels(automationUI, labManager.IsCropAutomated);
                }
            };
        }
    }

    private void HandleLocaleChanged(Locale locale)
    {
        RefreshCropNames();
        LoadButtonTexts(researchUI);
        LoadButtonTexts(automationUI);
    }

    private void LoadButtonTexts(ActionUI ui)
    {
        ui.actionLabel.GetLocalizedStringAsync().Completed += handle => ui.ActionText = handle.Result;
        ui.doneLabel.GetLocalizedStringAsync().Completed += handle => ui.DoneText = handle.Result;
    }
    
    /// <summary>
    /// Plays the progress loop only while the panel is open and research or automation is running.
    /// </summary>
    private void RefreshProgressLoop()
    {
        bool panelOpen = labPanel.activeSelf;

        SetLoop(panelOpen && labManager.IsResearching(),
            ref researchLoopPlaying, "ResearchSlider", "Research");
        
        SetLoop(panelOpen && labManager.IsAutomating(),
            ref automationLoopPlaying, "AutomationSlider", "Automation");
    }

    private static void SetLoop(bool shouldPlay, ref bool isPlaying, string soundName, string owner)
    {
        if (shouldPlay == isPlaying)
            return;

        isPlaying = shouldPlay;

        if (shouldPlay)
            SoundManager.Instance.StartLoop(soundName, owner);
        else
            SoundManager.Instance.StopLoop(owner);
    }

    /// <summary>
    /// Called when the research dropdown selection changes.
    /// </summary>
    public void OnResearchCropSelected(int index)
    {
        researchUI.SelectedCrop = labManager.GetCropAt(index);
    }

    /// <summary>
    /// Called when the automation dropdown selection changes.
    /// </summary>
    public void OnAutomationCropSelected(int index)
    {
        automationUI.SelectedCrop = labManager.GetCropAt(index);
    }

    /// <summary>
    /// Starts the selected crop research.
    /// </summary>
    public void StartResearchUI()
    {
        StartActionUI(researchUI, labManager.StartResearching);
    }

    /// <summary>
    /// Starts automation for the selected crop.
    /// </summary>
    public void StartAutomationUI()
    {
        if (!labManager.IsCropResearched(automationUI.SelectedCrop))
            return;
        
        StartActionUI(automationUI, labManager.StartAutomating);
    }

    /// <summary>
    /// Plays the UI transition before beginning an action.
    /// Prevents multiple start animations from running simultaneously.
    /// </summary>
    private void StartActionUI(ActionUI ui, Action<CropData> startAction)
    {
        if (ui.ActiveCoroutine != null)
            StopCoroutine(ui.ActiveCoroutine);

        ui.ActiveCoroutine = StartCoroutine(PlayStartActionRoutine(ui, startAction));

        // Prevent repeated clicks while animation is playing.
        ui.button.interactable = false;
    }

    /// <summary>
    /// Plays the button/slider transition before notifying the laboratory manager.
    /// </summary>
    private IEnumerator PlayStartActionRoutine(ActionUI ui, Action<CropData> startAction)
    {
        ui.buttonFade.Fade(false);

        yield return new WaitForSeconds(fadeWait);

        ui.slider.gameObject.SetActive(true);
        ui.sliderFade.Fade(true);

        yield return new WaitForSeconds(fadeWait);

        // Begin the actual laboratory action.
        startAction(ui.SelectedCrop);
        ui.ActiveCoroutine = null;
    }

    //==========================================================================
    // UI Updates
    //==========================================================================

    public void ToggleLabPanel()
    {
        labPanel.SetActive(!labPanel.activeSelf);
        SoundManager.Instance.PlaySFX("TogglePanel");
    }

    /// <summary>
    /// Updates all research-related UI elements.
    /// </summary>
    private void UpdateResearchUI()
    {
        ActionUI ui = researchUI;

        // No crop selected yet.
        if (!ui.SelectedCrop)
            return;

        List<CostEntry> costs = ui.SelectedCrop.ResearchCost;
        bool canAfford = resourceManager.CanAfford(costs);
        bool isDone = labManager.IsCropResearched(ui.SelectedCrop);
        bool isDoing = labManager.IsResearching();

        if (isDone || isDoing)
        {
            for (int i = 0; i < researchCostSlots.Count; i++)
                researchCostSlots[i].slotRoot.SetActive(false);
        }
        else
        {
            for (int i = 0; i < costs.Count; i++)
            {
                CostEntry entry = costs[i];
                CostSlotUI slot = researchCostSlots[i];

                slot.slotRoot.SetActive(true);
                slot.slotIcon.sprite = entry.type == ResourceType.Coin ? coinIcon : entry.crop.GrowthSprites[^1];

                bool entryAffordable = resourceManager.HasEnough(entry);
                string color = entryAffordable ? "green" : "red";
                string currentAmount = StatsFormatter.FormatNumber(entry.type == ResourceType.Coin ? resourceManager.Coins : resourceManager.GetCropCount(entry.crop));
                slot.slotText.text = $"<color={color}>{currentAmount}</color>/{entry.amount}";
            }
        }

        for (int i = costs.Count; i < researchCostSlots.Count; i++)
            researchCostSlots[i].slotRoot.SetActive(false);

        ui.button.interactable = canAfford && !isDoing && !isDone && ui.ActiveCoroutine == null;

        if (!isDoing)
        {
            // Idle state.
            ui.slider.value = 0f;

            if (ui.ActiveCoroutine == null)
            {
                ui.slider.gameObject.SetActive(false);
                ui.button.gameObject.SetActive(true);
                
                if (!isDone)
                    ui.buttonFade.SetInstant(true);
            }

            ui.dropdown.interactable = ui.ActiveCoroutine == null;

            if (isDone)
            {
                ui.rainbowText.enabled = true;
                if (!ui.CompleteFadePlayed)
                {
                    if(ui.button.gameObject.activeInHierarchy)
                        ui.buttonFade.Fade(true);
                    else
                        ui.buttonFade.SetInstant(true);
                    ui.CompleteFadePlayed = true;
                }

                ui.buttonText.text = ui.DoneText ?? "RESEARCHED";
            }
            else
            {
                ui.rainbowText.enabled = false;
                ui.buttonText.color = Color.white;
                ui.buttonText.text = ui.ActionText ?? "RESEARCH";
            }
        }
        else
        {
            // Active research state.
            ui.rainbowText.enabled = false;
            ui.CompleteFadePlayed = false;
            ui.dropdown.interactable = false;
            ui.button.gameObject.SetActive(false);
            ui.slider.gameObject.SetActive(true);
            ui.slider.value = labManager.GetResearchProgress() * 100f;
            ui.sliderText.text = $"{ui.slider.value:F3}%";
        }
    }

    /// <summary>
    /// Updates all automation-related UI elements.
    /// </summary>
    private void UpdateAutomationUI()
    {
        ActionUI ui = automationUI;

        if (!ui.SelectedCrop)
            return;

        List<CostEntry> costs = ui.SelectedCrop.AutomationCost;
        bool canAfford = resourceManager.CanAfford(costs);
        bool isDoing = labManager.IsAutomating();
        bool isDone = labManager.IsCropAutomated(ui.SelectedCrop);

        if (isDoing || isDone)
        {
            for (int i = 0; i < costs.Count; i++)
                automationCostSlots[i].slotRoot.SetActive(false);
        }
        else
        {
            for (int i = 0; i < costs.Count; i++)
            {
                CostEntry entry = costs[i];
                CostSlotUI slot = automationCostSlots[i];

                slot.slotRoot.SetActive(true);
                slot.slotIcon.sprite = entry.type == ResourceType.Coin ? coinIcon : entry.crop.GrowthSprites[^1];

                bool entryAffordable = resourceManager.HasEnough(entry);
                string color = entryAffordable ? "green" : "red";
                string currentAmount = StatsFormatter.FormatNumber(entry.type == ResourceType.Coin ? resourceManager.Coins : resourceManager.GetCropCount(entry.crop));
                slot.slotText.text = $"<color={color}>{currentAmount}</color>/{StatsFormatter.FormatNumber(entry.amount)}";
            }
        }

        for (int i = costs.Count; i < automationCostSlots.Count; i++)
            automationCostSlots[i].slotRoot.SetActive(false);

        ui.button.interactable = canAfford && !isDoing && !isDone && ui.ActiveCoroutine == null;

        if (!isDoing)
        {
            // Idle state.
            ui.slider.value = 0f;

            if (ui.ActiveCoroutine == null)
            {
                ui.slider.gameObject.SetActive(false);
                ui.button.gameObject.SetActive(true);

                if (!isDone)
                    ui.buttonFade.SetInstant(true);
            }

            ui.dropdown.interactable = ui.ActiveCoroutine == null;

            if (isDone)
            {
                ui.rainbowText.enabled = true;
                if (!ui.CompleteFadePlayed)
                {
                    if (ui.button.gameObject.activeInHierarchy)
                        ui.buttonFade.Fade(true);
                    else
                        ui.buttonFade.SetInstant(true);
                    
                    ui.CompleteFadePlayed = true;
                }
                ui.warningText.SetActive(false);
                ui.buttonText.text = ui.DoneText ?? "AUTOMATED";
            }
            else
            {
                ui.warningText.SetActive(true);
                ui.rainbowText.enabled = false;
                ui.buttonText.color = Color.white;
                ui.buttonText.text = ui.ActionText ?? "AUTOMATE";
            }
        }
        else
        {
            // Active automation state.
            ui.rainbowText.enabled = false;
            ui.CompleteFadePlayed = false;
            ui.dropdown.interactable = false;
            ui.button.gameObject.SetActive(false);
            ui.slider.gameObject.SetActive(true);
            ui.slider.value = labManager.GetAutomationProgress() * 100f;
            ui.sliderText.text = $"{ui.slider.value:F3}%";
        }
    }
}