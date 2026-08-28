using System;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeNodeUI : MonoBehaviour
{
    [SerializeField] private UpgradeManager upgradeManager;
    
    [SerializeField] private UpgradeData upgradeDataSO;
    [SerializeField] private Button upgradeNodeButton;
    [SerializeField] private Image upgradeIcon;
    [SerializeField] private Image boughtOutline;

    private void Start()
    {
        Refresh();
    }

    public void TryUnlockNode()
    {
        upgradeManager.UnlockUpgrade(upgradeDataSO);
    }

    public void Refresh()
    {
        switch (upgradeManager.GetUpgradeState((upgradeDataSO)))
        {
            case UpgradeState.Locked:
                SetVisualState(false, false, 0.4f);
                break;
            case UpgradeState.Available:
                SetVisualState(true, false, 1f);
                break;
            case UpgradeState.Bought:
                SetVisualState(true, true, 1f);
                break;
        }
    }

    private void SetVisualState(bool buttonInteractable, bool boughtOutlineEnabled, float alpha)
    {
        upgradeNodeButton.interactable = buttonInteractable;
        boughtOutline.enabled = boughtOutlineEnabled;
        Color c = upgradeIcon.color;
        c.a = alpha;
        upgradeIcon.color = c;
    }

}
