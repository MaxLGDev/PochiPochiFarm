using UnityEngine;

public enum RequiredCropState
{
    None,
    Unlocked,
    Researched,
    Automated
}

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrades/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public string UpgradeName;
    public Sprite Sprite;
    public int UnlockCost;
    
    public UpgradeData PreviousUpgrade;
    public RequiredCropState CropState;
    public CropData TargetCrop;
}
