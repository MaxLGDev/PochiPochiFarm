using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the data assigned to a single tile in the farm.
/// </summary>
[System.Serializable]
public class FarmInfo
{
    // --- Tile Data ---

    // Grid position of the tile.
    public Vector2Int position;

    // Crop assigned to this tile.
    public CropData cropData;

    // Ground sprite displayed beneath the crop.
    public Sprite tileSprite;
}

/// <summary>
/// Stores the complete layout of the farm.
/// Each entry represents one tile in the grid.
/// </summary>
[CreateAssetMenu(menuName = "Farm/Farm Layout")]
public class FarmLayout : ScriptableObject
{
    // --- Farm Tiles ---
    public List<FarmInfo> tiles;
}