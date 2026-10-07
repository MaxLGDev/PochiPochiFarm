
using UnityEngine;
using TMPro;
using UnityEngine.Localization;

[ExecuteAlways]
public class LocalizedFont : MonoBehaviour
{
    [SerializeField] private LocalizedTmpFont font;
    [SerializeField] private TMP_Text label;

    private void OnEnable()
    {
        font.AssetChanged += Apply;
    }

    private void OnDisable()
    {
        font.AssetChanged -= Apply;
    }

    private void Apply(TMP_FontAsset newFont)
    {
        label.font = newFont;
    }
}
