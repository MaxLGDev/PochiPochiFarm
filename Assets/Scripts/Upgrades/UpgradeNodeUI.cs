using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Serialization;

public class UpgradeNodeUI : MonoBehaviour
{
    public event Action<UpgradeData> OnNodeClicked;
    
    private UpgradeState upgradeState;
    [SerializeField] private UpgradeData upgradeDataSo;
    [SerializeField] private Button upgradeNodeButton;
    [FormerlySerializedAs("upgradeIcon")] [SerializeField] private Image nodeIcon;
    [SerializeField] private Image boughtOutline;
    [SerializeField] private Image nodeBorder;
    [SerializeField] private Image nodeBackground;
    [SerializeField] private Image nodeOverlay;
    [SerializeField] private TMP_Text upgradeNameText;

    [SerializeField] private Color lockedColor;
    [SerializeField] private Color availableColor;
    [SerializeField] private Color boughtColor;
    
    public RectTransform RectTransform => (RectTransform)transform;

    public UpgradeData UpgradeDataSo => upgradeDataSo;

    public void TryUnlockNode() => OnNodeClicked?.Invoke(upgradeDataSo);

    public void Refresh(UpgradeState state)
    {
        switch (state)
        {
            case UpgradeState.Locked:
                SetVisualState(false, false, true,0.4f, lockedColor);
                break;
            case UpgradeState.Available:
                SetVisualState(true, false, true, 1f, availableColor);
                break;
            case UpgradeState.Bought:
                SetVisualState(true, true, false, 1f, boughtColor);
                break;
        }
    }

    private void SetVisualState(bool buttonInteractable, bool boughtOutlineEnabled, bool overlayEnabled, float alpha, Color textColor)
    {
        upgradeNodeButton.interactable = buttonInteractable;
        boughtOutline.enabled = boughtOutlineEnabled;
        nodeOverlay.enabled = overlayEnabled;
        SetColorOf(nodeIcon, alpha);
        SetColorOf(nodeBackground, alpha);

        nodeIcon.sprite = upgradeDataSo.Sprite;

        upgradeNameText.color = textColor;
    }

    private static void SetColorOf(Image image, float alpha)
    {
        var c = image.color;
        c.a = alpha;
        image.color = c;
    }

}
