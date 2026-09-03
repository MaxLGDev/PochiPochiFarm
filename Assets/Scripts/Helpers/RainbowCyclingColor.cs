using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RainbowCyclingColor : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private Image icon;
    [SerializeField] private float cyclingSpeed = 1f;
    private float hue;

    private void Update()
    {
        CycleRainbowColor();
    }

    private void CycleRainbowColor()
    {
        if (!text && !icon)
            return;

        var rainbowColor = Color.HSVToRGB(hue, 1f, 1f);
        hue += cyclingSpeed * Time.deltaTime;
        hue %= 1f;

        if(text)
            text.color = rainbowColor;
        
        if(icon)
            icon.color = rainbowColor;
    }
}
