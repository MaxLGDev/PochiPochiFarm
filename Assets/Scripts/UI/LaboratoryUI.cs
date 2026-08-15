using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
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
        [NonSerialized] public CropData selectedCrop;

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
        [NonSerialized] public Coroutine activeCoroutine;

        [NonSerialized] public bool completeFadePlayed;

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

    [SerializeField] private List<CostSlotUI> researchCostSlots;
    [SerializeField] private List<CostSlotUI> automationCostSlots;

    [SerializeField] private GameObject labPanel;

    // Coin icon used for both research and automation costs.
    [SerializeField] private Sprite coinIcon;

    // UI for the research panel.
    [SerializeField] private ActionUI researchUI;

    // UI for the automation panel.
    [SerializeField] private ActionUI automationUI;

    [SerializeField] private float fadeWait = 0.2f;

    private void Start()
    {
        labPanel.SetActive(false);

        OnResearchCropSelected(0);
        OnAutomationCropSelected(0);
    }

    private void Update()
    {
        // Refresh both interfaces every frame.
        UpdateResearchUI();
        UpdateAutomationUI();
    }

    /// <summary>
    /// Called when the research dropdown selection changes.
    /// </summary>
    public void OnResearchCropSelected(int index)
    {
        researchUI.selectedCrop = labManager.GetCropAt(index);
    }

    /// <summary>
    /// Called when the automation dropdown selection changes.
    /// </summary>
    public void OnAutomationCropSelected(int index)
    {
        automationUI.selectedCrop = labManager.GetCropAt(index);
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
        StartActionUI(automationUI, labManager.StartAutomating);
    }

    /// <summary>
    /// Plays the UI transition before beginning an action.
    /// Prevents multiple start animations from running simultaneously.
    /// </summary>
    private void StartActionUI(ActionUI ui, Action<CropData> startAction)
    {
        if (ui.activeCoroutine != null)
            StopCoroutine(ui.activeCoroutine);

        ui.activeCoroutine = StartCoroutine(PlayStartActionRoutine(ui, startAction));

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
        startAction(ui.selectedCrop);
        ui.activeCoroutine = null;
    }

    //==========================================================================
    // UI Updates
    //==========================================================================

    public void ToggleLabPanel() => labPanel.SetActive(!labPanel.activeSelf);

    /// <summary>
    /// Updates all research-related UI elements.
    /// </summary>
    private void UpdateResearchUI()
    {
        ActionUI ui = researchUI;

        // No crop selected yet.
        if (ui.selectedCrop == null)
            return;

        List<CostEntry> costs = ui.selectedCrop.ResearchCost;
        bool canAfford = resourceManager.CanAfford(costs);
        bool isDone = labManager.IsCropResearched(ui.selectedCrop);
        bool isDoing = labManager.IsResearching();

        if(isDone || isDoing)
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

        ui.button.interactable = canAfford && !isDoing && !isDone;

        if (!isDoing)
        {
            // Idle state.
            ui.slider.value = 0f;

            if (ui.activeCoroutine == null)
            {
                ui.slider.gameObject.SetActive(false);
                ui.button.gameObject.SetActive(true);
            }

            ui.dropdown.interactable = true;

            if (isDone)
            {
                ui.rainbowText.enabled = true;
                if(!ui.completeFadePlayed)
                {
                    ui.buttonFade.Fade(true);
                    ui.completeFadePlayed = true;
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
            ui.completeFadePlayed = false;
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

        if (ui.selectedCrop == null)
            return;

        List<CostEntry> costs = ui.selectedCrop.AutomationCost;
        bool canAfford = resourceManager.CanAfford(costs);
        bool isDoing = labManager.IsAutomating();
        bool isDone = labManager.IsCropAutomated(ui.selectedCrop);

        if(isDoing || isDone)
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
                slot.slotIcon.sprite = entry.type == ResourceType.Coin ? coinIcon : entry.crop.GrowthSprites[entry.crop.GrowthSprites.Length - 1];

                bool entryAffordable = resourceManager.HasEnough(entry);
                string color = entryAffordable ? "green" : "red";
                int currentAmount = entry.type == ResourceType.Coin ? resourceManager.Coins : resourceManager.GetCropCount(entry.crop);
                slot.slotText.text = $"<color={color}>{currentAmount}</color>/{entry.amount}";
            }
        }

        for (int i = costs.Count; i < automationCostSlots.Count; i++)
            automationCostSlots[i].slotRoot.SetActive(false);

        ui.button.interactable = canAfford && !isDoing && !isDone;

        if (!isDoing)
        {
            // Idle state.
            ui.slider.value = 0f;
             
            if (ui.activeCoroutine == null)
            {
                ui.slider.gameObject.SetActive(false);
                ui.button.gameObject.SetActive(true);
            }

            ui.dropdown.interactable = true;

            if (isDone)
            {
                ui.rainbowText.enabled = true;
                if (!ui.completeFadePlayed)
                {
                    ui.buttonFade.Fade(true);
                    ui.completeFadePlayed = true;
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
            ui.dropdown.interactable = false;
            ui.button.gameObject.SetActive(false);
            ui.slider.gameObject.SetActive(true);
            ui.slider.value = labManager.GetAutomationProgress() * 100f;
            ui.sliderText.text = $"{ui.slider.value:F3}%";
        }
    }
}