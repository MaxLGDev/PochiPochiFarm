using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

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

        /// <summary>
        /// Reference to the currently running UI animation coroutine.
        /// Allows restarting without duplicates.
        /// </summary>
        [NonSerialized] public Coroutine ActiveCoroutine;

        [NonSerialized] public bool CompleteFadePlayed;

        public RainbowCyclingColor rainbowText;

        /// <summary>
        /// Crops currently listed in the dropdown (same order as its options).
        /// </summary>
        [NonSerialized] public List<CropData> ListedCrops;

        /// <summary>
        /// Original dropdown options from the scene, in the same order as
        /// LaboratoryManager.ResearchableCrops. Used to rebuild the list.
        /// </summary>
        [NonSerialized] public List<TMP_Dropdown.OptionData> AllOptions;
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

    [SerializeField] private Button laboratoryButton;

    [SerializeField] private GameObject labPanel;

    // Coin icon used for both research and automation costs.
    [SerializeField] private Sprite coinIcon;

    // UI for the research panel.
    [SerializeField] private ActionUI researchUI;

    // UI for the automation panel.
    [SerializeField] private ActionUI automationUI;

    [SerializeField] private float fadeWait = 0.2f;

    private bool laboratoryUnlocked;
    private bool researchLoopPlaying;
    private bool automationLoopPlaying;

    // Reused every frame to avoid allocating while checking the dropdowns.
    private readonly List<CropData> wantedCrops = new();

    private void Start()
    {
        labPanel.SetActive(false);
        SetLaboratoryUnlocked(journalManager.IsChapter1Claimed());

        researchUI.ListedCrops = new List<CropData>();
        researchUI.AllOptions = new List<TMP_Dropdown.OptionData>(researchUI.dropdown.options);
        automationUI.ListedCrops = new List<CropData>();
        automationUI.AllOptions = new List<TMP_Dropdown.OptionData>(automationUI.dropdown.options);

        researchUI.dropdown.onValueChanged.AddListener(OnResearchCropSelected);
        automationUI.dropdown.onValueChanged.AddListener(OnAutomationCropSelected);

        RefreshDropdown(researchUI, labManager.IsCropResearched);
        RefreshDropdown(automationUI, labManager.IsCropAutomated);
    }

    private void HandleJournalChapter1Claimed()
    {
        SetLaboratoryUnlocked(true);
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
    }

    private void OnDisable()
    {
        journalManager.OnChapter1Claimed -= HandleJournalChapter1Claimed;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopLoop("Research");
            SoundManager.Instance.StopLoop("Automation");
        }

        researchLoopPlaying = automationLoopPlaying = false;

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
        SelectListedCrop(researchUI, index);
    }

    /// <summary>
    /// Called when the automation dropdown selection changes.
    /// </summary>
    public void OnAutomationCropSelected(int index)
    {
        SelectListedCrop(automationUI, index);
    }

    private static void SelectListedCrop(ActionUI ui, int index)
    {
        ui.SelectedCrop = index >= 0 && index < ui.ListedCrops.Count ? ui.ListedCrops[index] : null;
    }

    /// <summary>
    /// Keeps the dropdown limited to crops that are not done yet
    /// (not researched for research, not automated for automation).
    /// Rebuilds the options only when that set changes, so it also covers
    /// completions, loading a save and crops that start researched.
    /// </summary>
    private void RefreshDropdown(ActionUI ui, Func<CropData, bool> isDone)
    {
        IReadOnlyList<CropData> allCrops = labManager.ResearchableCrops;

        wantedCrops.Clear();
        for (int i = 0; i < allCrops.Count; i++)
        {
            if (!isDone(allCrops[i]))
                wantedCrops.Add(allCrops[i]);
        }

        if (SameCrops(wantedCrops, ui.ListedCrops) && ui.dropdown.options.Count == wantedCrops.Count)
            return;

        CropData previous = ui.SelectedCrop;

        ui.ListedCrops.Clear();
        ui.ListedCrops.AddRange(wantedCrops);

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        int selected = 0;

        for (int i = 0; i < ui.ListedCrops.Count; i++)
        {
            CropData crop = ui.ListedCrops[i];

            if (crop == previous)
                selected = i;

            int sourceIndex = IndexOfCrop(allCrops, crop);
            options.Add(sourceIndex < ui.AllOptions.Count
                ? ui.AllOptions[sourceIndex]
                : new TMP_Dropdown.OptionData(crop.CropName));
        }

        ui.dropdown.options = options;
        ui.dropdown.SetValueWithoutNotify(selected);
        ui.dropdown.RefreshShownValue();

        SelectListedCrop(ui, selected);

        // Nothing left to pick: show a label instead of an empty caption.
        if (ui.ListedCrops.Count == 0)
            ui.dropdown.captionText.text = "All done";
    }

    private static bool SameCrops(List<CropData> a, List<CropData> b)
    {
        if (a.Count != b.Count)
            return false;

        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i])
                return false;
        }

        return true;
    }

    private static int IndexOfCrop(IReadOnlyList<CropData> crops, CropData crop)
    {
        for (int i = 0; i < crops.Count; i++)
        {
            if (crops[i] == crop)
                return i;
        }

        return -1;
    }

    /// <summary>
    /// Shown when every crop in the dropdown has been completed.
    /// </summary>
    private void ShowAllDone(ActionUI ui, List<CostSlotUI> costSlots, string label)
    {
        for (int i = 0; i < costSlots.Count; i++)
            costSlots[i].slotRoot.SetActive(false);

        ui.slider.gameObject.SetActive(false);
        ui.button.gameObject.SetActive(true);
        ui.buttonFade.SetInstant(true);
        ui.button.interactable = false;
        ui.dropdown.interactable = false;
        ui.rainbowText.enabled = true;
        ui.buttonText.text = label;

        if (ui.warningText != null)
            ui.warningText.SetActive(false);
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

        RefreshDropdown(ui, labManager.IsCropResearched);

        // Every crop is researched.
        if (ui.SelectedCrop == null)
        {
            ShowAllDone(ui, researchCostSlots, "ALL RESEARCHED");
            return;
        }

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
                slot.slotIcon.sprite = entry.type == ResourceType.Coin ? coinIcon : entry.crop.GrowthSprites[entry.crop.GrowthSprites.Length - 1];

                bool entryAffordable = resourceManager.HasEnough(entry);
                string color = entryAffordable ? "green" : "red";
                int currentAmount = entry.type == ResourceType.Coin ? resourceManager.Coins : resourceManager.GetCropCount(entry.crop);
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
                ui.buttonText.text = "RESEARCHED";
            }
            else
            {
                ui.rainbowText.enabled = false;
                ui.buttonText.color = Color.white;
                ui.buttonText.text = "RESEARCH";
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

        RefreshDropdown(ui, labManager.IsCropAutomated);

        // Every crop is automated.
        if (ui.SelectedCrop == null)
        {
            ShowAllDone(ui, automationCostSlots, "ALL AUTOMATED");
            return;
        }

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
                int currentAmount = entry.type == ResourceType.Coin ? resourceManager.Coins : resourceManager.GetCropCount(entry.crop);
                slot.slotText.text = $"<color={color}>{currentAmount}</color>/{entry.amount}";
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
                ui.buttonText.text = "AUTOMATED";
            }
            else
            {
                ui.warningText.SetActive(true);
                ui.rainbowText.enabled = false;
                ui.buttonText.color = Color.white;
                ui.buttonText.text = "AUTOMATE";
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
