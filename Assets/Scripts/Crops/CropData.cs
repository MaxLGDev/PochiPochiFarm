using System.Collections.Generic;
using UnityEngine;

// ============================================
// Enums
// ============================================

/// <summary>
/// Defines the different crop types available in the game.
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

/// <summary>
/// Defines the different resource types that can be used or earned.
/// </summary>
public enum ResourceType
{
    Coin,
    Crop
}


// ============================================
// Serializable Data
// ============================================

/// <summary>
/// Represents a resource required to purchase or unlock something.
/// </summary>
[System.Serializable]
public class CostEntry
{
    public ResourceType type;
    public CropData crop;
    public int amount;
}


// ============================================
// Crop Data
// ============================================

/// <summary>
/// Stores all data associated with a crop.
/// Used by gameplay systems to determine its appearance,
/// growth behaviour, costs, and rewards.
/// </summary>
[CreateAssetMenu(fileName = "New Crop Data", menuName = "Crops/Crop")]
public class CropData : ScriptableObject
{
    // --- Identity ---
    public string CropName;
    public CropType CropType;
    public bool startsResearched;


    // --- Research & Automation ---
    public List<CostEntry> ResearchCost;
    public List<CostEntry> AutomationCost;


    // --- Sprites ---
    [Header("Sprites")]

    // Sprite displayed for the crop's seed.
    public Sprite SeedSprite;

    // Sprites representing each stage of the crop's growth.
    public Sprite[] GrowthSprites;


    // --- Gameplay ---
    [Header("Gameplay")]

    // Time, in seconds, required for the crop to fully mature.
    public float GrowthTime;

    // Amount of water required before the crop can grow.
    public int RequiredWater;

    // Number of coins awarded when the crop is harvested.
    public int CoinYield;

    // Cost required to unlock the crop.
    public int UnlockCost;


    // --- Research ---
    // Time, in seconds, required to research and unlock the crop.
    public float ResearchDuration;


    // --- Automation ---
    // Time, in seconds, required to automate the crop.
    public float AutomationDuration;
}