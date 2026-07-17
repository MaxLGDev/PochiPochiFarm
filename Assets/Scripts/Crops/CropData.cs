using UnityEngine;

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

[CreateAssetMenu(fileName = "New Crop Data", menuName = "Crops/Crop")]
public class CropData : ScriptableObject
{
    public string cropName;
    public CropType cropType;

    public Sprite cropSprite; // On the farm
    public Sprite seedSprite; // In the shop

    public float growthTime; // Time in seconds for the crop to grow
    public float requiredWater; // Amount of water required for the crop to grow
    public float coinYield;
    public float unlockCost; // Cost to unlock the crop
}
