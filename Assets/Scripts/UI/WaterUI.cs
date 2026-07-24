using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the water display and passive regeneration slider.
/// </summary>
public class WaterUI : MonoBehaviour
{
    [SerializeField] private WaterManager waterManager;

    [SerializeField] private TMPro.TextMeshProUGUI waterText;
    [SerializeField] private Slider waterSlider;

    private void Awake()
    {
        UpdateWaterUI(waterManager.Water);

        waterSlider.maxValue = waterManager.PassiveWaterInterval;
        waterSlider.value = 0f;
    }

    private void OnEnable()
    {
        waterManager.OnWaterChanged += UpdateWaterUI;
    }

    private void OnDisable()
    {
        waterManager.OnWaterChanged -= UpdateWaterUI;
    }

    private void Update()
    {
        UpdateSliderUI();
    }

    //==========================================================================
    // UI
    //==========================================================================

    /// <summary>
    /// Refreshes the displayed water amount.
    /// </summary>
    private void UpdateWaterUI(int waterAmount)
    {
        if (waterAmount == 0)
        {
            waterText.text = $"<color=red>{waterAmount}/{waterManager.MaxWater}</color>";
        }
        else if (waterAmount == waterManager.MaxWater)
        {
            waterText.text = $"<color=green>{waterAmount}/{waterManager.MaxWater}</color>";
        }
        else
        {
            waterText.text = $"<color=yellow>{waterAmount}/{waterManager.MaxWater}</color>";
        }
    }

    /// <summary>
    /// Updates the passive water regeneration progress bar.
    /// </summary>
    private void UpdateSliderUI()
    {
        if (waterManager.Water == waterManager.MaxWater)
        {
            waterSlider.value = 0f;
            return;
        }

        waterSlider.value += Time.deltaTime;

        if (waterSlider.value >= waterManager.PassiveWaterInterval)
            waterSlider.value = 0f;
    }
}