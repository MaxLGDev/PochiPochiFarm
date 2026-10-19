using TMPro;
using UnityEngine;
using UnityEngine.Localization;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedFont : MonoBehaviour
{
    [SerializeField] private LocalizedAsset<TMP_FontAsset> localizedFont;

    private TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        // Fires as soon as the font is loaded, and again whenever the locale changes.
        localizedFont.AssetChanged += OnFontChanged;
    }

    private void OnDisable()
    {
        localizedFont.AssetChanged -= OnFontChanged;
    }

    private void OnFontChanged(TMP_FontAsset font)
    {
        if (font != null)
            text.font = font;
    }
}