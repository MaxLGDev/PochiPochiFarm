using TMPro;
using UnityEngine;

public class RainbowCyclingColor : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float cyclingSpeed = 1f;
    private float hue;

    private void Update()
    {
        CycleRainbowColor();
    }

    private void CycleRainbowColor()
    {
        if(text == null)
        {
            Debug.LogWarning($"No text assigned.");
            return;
        }

        Color rainbowColor = Color.HSVToRGB(hue, 1f, 1f);
        hue += cyclingSpeed * Time.deltaTime;
        hue %= 1f;

        text.color = rainbowColor;
    }
}
