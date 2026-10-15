using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UpgradeNodeUI : MonoBehaviour
{
    public event Action<UpgradeData> OnNodeClicked;
    
    [SerializeField] private UpgradeData upgradeDataSo;
    [SerializeField] private Button upgradeNodeButton;
    [SerializeField] private Image nodeIcon;
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

    private void Start()
    {
        nodeIcon.sprite = upgradeDataSo.Sprite;
        upgradeNameText.text =  upgradeDataSo.UpgradeName;
        RefreshEffectText();
    }

    public void TryUnlockNode() => OnNodeClicked?.Invoke(upgradeDataSo);

    public void Refresh(UpgradeNodeInfo info)
    {
        switch (info.State)
        {
            case UpgradeState.Locked: 
                ShowLocked();
                break;
            case UpgradeState.Available: 
                ShowAvailable();
                break;
            case UpgradeState.Bought:
                ShowBought();
                break;
        }

        RefreshCostDisplay(info.State, info.IsAffordable);
        RefreshRequirementDisplay(info.State, info.RequirementMet);
    }

    private void ShowLocked()
    {
        upgradeNodeButton.interactable = false;
        boughtOutline.enabled = false;
        nodeOverlay.enabled = true;
        SetAlpha(0.4f);
        upgradeNameText.color = lockedColor;
    }

    private void ShowAvailable()
    {
        upgradeNodeButton.interactable = true;
        boughtOutline.enabled = false;
        nodeOverlay.enabled = true;
        SetAlpha(1f);
        upgradeNameText.color = availableColor;
    }

    private void ShowBought()
    {
        upgradeNodeButton.interactable = true;
        boughtOutline.enabled = true;
        nodeOverlay.enabled = false;
        SetAlpha(1f);
        upgradeNameText.color = boughtColor;
    }

    private void SetAlpha(float alpha)
    {
        SetColorOf(nodeIcon, alpha);
        SetColorOf(nodeBackground, alpha);
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
