using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Types of crops available in the game.
/// </summary>
public enum CropType
{
    Dirt,
    Blackberry,
    Cabbage,
    Carrots,
    Cauliflower,
    Eggplant,
    Leeks,
    Pumpkin,
    Radish,
    Raspberry,
    Zucchini,
    Strawberry,
    Tomato
}

public enum ResourceType
{
    Coin,
    Crop
}

[System.Serializable]
public class CostEntry
{
    public ResourceType type;
    public CropData crop;
    public int amount;
}

/// <summary>
/// Stores all data related to a crop.
/// Used by tiles to determine appearance, growth, and rewards.
/// </summary>
[CreateAssetMenu(fileName = "New Crop Data", menuName = "Crops/Crop")]
public class CropData : ScriptableObject
{
    public string CropName;
    public CropType CropType;
    public bool startsResearched;

    public List<CostEntry> ResearchCost;
    public List<CostEntry> AutomationCost;

    [Header("Sprites")]

    // Sprite displayed in the shop.
    public Sprite SeedSprite;

    // Sprites used throughout the crop's growth stages.
    public Sprite[] GrowthSprites;

    [Header("Gameplay")]

    // Time in seconds for the crop to fully mature.
    public float GrowthTime;

    // Water required before the crop can grow.
    public int RequiredWater;

    // Coins earned when harvesting.
    public int CoinYield;

    // Cost to unlock this crop.
    public int UnlockCost;

    // Duration for the research
    public float ResearchDuration;

    // Duration for the automation
    public float AutomationDuration;
}