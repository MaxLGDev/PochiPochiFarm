using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RainbowCyclingColor : MonoBehaviour
{
    // --- References ---
    [SerializeField] private TMP_Text text;
    [SerializeField] private Image icon;

    // --- Settings ---
    [SerializeField] private float cyclingSpeed = 1f;

    // --- State ---
    private float hue;


    // ==============================
    // Unity Lifecycle
    // ==============================

    private void Update()
    {
        CycleRainbowColor();
    }


    // ==============================
    // Rainbow Color
    // ==============================

    private void CycleRainbowColor()
    {
        // Nothing to update if neither the text nor icon is assigned.
        if (!text && !icon)
            return;

        var rainbowColor = Color.HSVToRGB(hue, 1f, 1f);

        hue += cyclingSpeed * Time.deltaTime;
        hue %= 1f;

        if (text)
            text.color = rainbowColor;

        if (icon)
            icon.color = rainbowColor;
    }
}