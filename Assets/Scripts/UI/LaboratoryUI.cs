using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the laboratory UI, including research and automation progress.
/// </summary>
public class LaboratoryUI : MonoBehaviour
{
    [SerializeField] private LaboratoryManager labManager;

    [SerializeField] private Slider researchSlider;
    [SerializeField] private TMP_Text researchSliderText;

    [SerializeField] private Slider automationSlider;
    [SerializeField] private TMP_Text automationSliderText;

    private void Update()
    {
        UpdateResearchSliderUI();
        UpdateAutomationSliderUI();
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