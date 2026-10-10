using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Serialization;

public class UpgradeNodeUI : MonoBehaviour
{
    public event Action<UpgradeData> OnNodeClicked;
    
    [SerializeField] private UpgradeData upgradeDataSo;
    [SerializeField] private Button upgradeNodeButton;
    [FormerlySerializedAs("upgradeIcon")] [SerializeField] private Image nodeIcon;
    [SerializeField] private Image boughtOutline;
    [SerializeField] private Image nodeBackground;
    [SerializeField] private Image nodeOverlay;
    [SerializeField] private TMP_Text upgradeNameText;
    [SerializeField] private TMP_Text upgradeCostText;
    [SerializeField] private TMP_Text upgradeEffectText;
    [SerializeField] private TMP_Text upgradePurchasedText;
    [SerializeField] private Image goldIcon;
    [SerializeField] private TMP_Text requirementText;

    [SerializeField] private Color lockedColor;
    [SerializeField] private Color availableColor;
    [SerializeField] private Color boughtColor;
    [SerializeField] private Color requirementMetColor = Color.green;
    [SerializeField] private Color requirementNotMetColor = Color.red;

    [SerializeField] private Color affordableColor;
    [SerializeField] private Color unaffordableColor;
    
    public RectTransform RectTransform => (RectTransform)transform;

    public RectTransform ConnectorAnchor => nodeIcon.rectTransform;

    public UpgradeData UpgradeDataSo => upgradeDataSo;

    public void TryUnlockNode() => OnNodeClicked?.Invoke(upgradeDataSo);

    public void Refresh(UpgradeState state, bool isAffordable, bool cropRequirementMet)
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

        RefreshCostDisplay(state, isAffordable);
        RefreshRequirementDisplay(state, cropRequirementMet);
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
        upgradeNameText.text = upgradeDataSo.UpgradeName;
        
        RefreshEffectText();
    }

    private static void SetColorOf(Image image, float alpha)
    {
        var c = image.color;
        c.a = alpha;
        image.color = c;
    }

    private void RefreshCostDisplay(UpgradeState state, bool isAffordable)
    {
        bool isBought = state == UpgradeState.Bought;
        upgradeCostText.text = upgradeDataSo.UnlockCost.ToString();
        
        upgradeCostText.color = isAffordable ? affordableColor : unaffordableColor;
        upgradePurchasedText.color = boughtColor;

        upgradeCostText.gameObject.SetActive(!isBought);
        upgradePurchasedText.gameObject.SetActive(isBought);
        goldIcon.gameObject.SetActive(!isBought);
    }

    private void RefreshRequirementDisplay(UpgradeState state, bool met)
    {
        bool hasRequirement = upgradeDataSo.TargetCrop != null && upgradeDataSo.CropState != RequiredCropState.None;

        bool show = hasRequirement && state != UpgradeState.Bought;
        requirementText.gameObject.SetActive(show);

        if (!show)
            return;
        
        requirementText.text = $"{upgradeDataSo.TargetCrop.CropName} {upgradeDataSo.CropState}";
        requirementText.color = met ? requirementMetColor : requirementNotMetColor;
    }

    private void RefreshEffectText()
    {
        upgradeEffectText.text = upgradeDataSo.EffectType switch
        {
            EffectType.MaxCoins or EffectType.MaxWater or EffectType.WaterRegenPower or EffectType.ClickMultiplier
                or EffectType.YieldMultiplier => $"{upgradeDataSo.UpgradeEffect} +{upgradeDataSo.EffectAmount}",
            
            EffectType.AutomationSpeed or EffectType.ResearchSpeed =>
                $"{upgradeDataSo.UpgradeEffect} +{100 * upgradeDataSo.EffectAmount}%",
            
            EffectType.SkipWaterChance => $"{upgradeDataSo.UpgradeEffect} +{upgradeDataSo.EffectAmount}%",
            
            EffectType.WaterRegenSpeed => $"{upgradeDataSo.UpgradeEffect} -{upgradeDataSo.EffectAmount}s",
            
            EffectType.None => "No effect",
            
            _ => ReportUnhandledEffect()
        };
    }

    private string ReportUnhandledEffect()
    {
        Debug.LogWarning($"Unhandled effect for {upgradeDataSo.UpgradeName}");

        return string.Empty;
    }
}
