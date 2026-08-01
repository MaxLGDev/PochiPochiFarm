using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the laboratory UI, including research and automation progress.
/// </summary>
public class LaboratoryUI : MonoBehaviour
{
    private CropData selectedCrop;

    [SerializeField] private LaboratoryManager labManager;
    [SerializeField] private ResourceManager resourceManager;

    [SerializeField] private Sprite coinIcon;
    [SerializeField] private Image neededCropIcon;
    [SerializeField] private TMP_Text neededCropAmount;
    [SerializeField] private Button researchButton;

    [SerializeField] private Slider researchSlider;
    [SerializeField] private TMP_Text researchSliderText;

    [SerializeField] private Slider automationSlider;
    [SerializeField] private TMP_Text automationSliderText;


    private void Update()
    {
        UpdateResearchSliderUI();
        UpdateAutomationSliderUI();
    }

    public void OnCropSelected(int index)
    {
        Debug.Log($"Dropdown fired with index {index}");
        CropData crop = labManager.GetCropAt(index);
        selectedCrop = crop;
        RefreshInfobox();
    }

    private void RefreshInfobox()
    {
        neededCropIcon.sprite = coinIcon;

        bool canAfford = resourceManager.HasEnoughCoinsForResearch(selectedCrop.ResearchCost);

        string color = canAfford ? "green" : "red";
        neededCropAmount.text = $"<color={color}>{resourceManager.Coins}</color>/{selectedCrop.ResearchCost}";

        researchButton.interactable = canAfford;
        Debug.Log(selectedCrop.name + " " + selectedCrop.ResearchCost + " " + canAfford);
    }

    //==========================================================================
    // UI
    //==========================================================================

    /// <summary>
    /// Refreshes the research progress bar.
    /// </summary>
    private void UpdateResearchSliderUI()
    {
        if (!labManager.IsResearching())
        {
            researchSlider.value = 0f;
            researchSliderText.text = "NO RESEARCH";
        }
        else
        {
            researchSlider.value = labManager.GetResearchProgress() * 100f;
            researchSliderText.text = $"{researchSlider.value:F1}%";
        }
    }

    /// <summary>
    /// Refreshes the automation progress bar.
    /// </summary>
    private void UpdateAutomationSliderUI()
    {
        if (!labManager.IsAutomating())
        {
            automationSlider.value = 0f;
            automationSliderText.text = "NO AUTOMATION";
        }
        else
        {
            automationSlider.value = labManager.GetAutomationProgress() * 100f;
            automationSliderText.text = $"{automationSlider.value:F1}%";
        }
    }
}