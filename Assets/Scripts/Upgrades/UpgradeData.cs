using UnityEngine;
using UnityEngine.Localization;

public enum RequiredCropState
{
    None,
    Unlocked,
    Researched,
    Automated
}

public enum EffectType
{
    None,
    MaxCoins,
    MaxWater,
    WaterRegenPower,
    WaterRegenSpeed,
    SkipWaterChance,
    ClickMultiplier,
    YieldMultiplier,
    ResearchSpeed,
    AutomationSpeed
}

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrades/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public LocalizedString LocalizedName;
    public LocalizedString LocalizedEffect;
    public string NameSuffix;
    
    public Sprite Sprite;
    public int UnlockCost;
    
    public UpgradeData PreviousUpgrade;
    public RequiredCropState CropState;
    public CropData TargetCrop;
    
    public EffectType EffectType;
    public int EffectAmount;
}
