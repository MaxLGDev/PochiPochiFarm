using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UpgradeNodeUI : MonoBehaviour
{
    public event Action<UpgradeData> OnNodeClicked;
    
    private UpgradeState upgradeState;
    [SerializeField] private UpgradeData upgradeDataSO;
    [SerializeField] private Button upgradeNodeButton;
    [SerializeField] private Image upgradeIcon;
    [SerializeField] private Image boughtOutline;
    [SerializeField] private TMP_Text upgradeNameText;

    [SerializeField] private Color lockedColor;
    [SerializeField] private Color availableColor;
    [SerializeField] private Color boughtColor;

    public UpgradeData UpgradeDataSO => upgradeDataSO;

    public void TryUnlockNode()
    {
        OnNodeClicked?.Invoke(upgradeDataSO);
    }

    public void Refresh(UpgradeState upgradeState)
    {
        switch (upgradeState)
        {
            case UpgradeState.Locked:
                SetVisualState(false, false, 0.4f, lockedColor);
                break;
            case UpgradeState.Available:
                SetVisualState(true, false, 1f, availableColor);
                break;
            case UpgradeState.Bought:
                SetVisualState(true, true, 1f, boughtColor);
                break;
        }
    }

    private void SetVisualState(bool buttonInteractable, bool boughtOutlineEnabled, float alpha, Color textColor)
    {
        upgradeNodeButton.interactable = buttonInteractable;
        boughtOutline.enabled = boughtOutlineEnabled;
        Color c = upgradeIcon.color;
        c.a = alpha;
        upgradeIcon.color = c;

        upgradeNameText.color = textColor;
    }

}
