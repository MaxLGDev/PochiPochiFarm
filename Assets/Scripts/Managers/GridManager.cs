using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class GridManager : MonoBehaviour
{

    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private FarmLayout farmLayout;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private List<ZoneData> zonesData;

    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;

    [SerializeField] private float cellSize = 1f;

    //TEMPORARY PLACEHOLDER FOR TEST
    [SerializeField] private int unlockCost = 10;

    private Tile[,] grid;

    private List<ZoneRuntime> zones;


    private void Awake()
    {
        if (farmLayout == null)
        {
            Debug.LogError("FarmLayout is not assigned");
            return;
        }

        grid = new Tile[width, height];

        zones = zonesData.Select(z => new ZoneRuntime(z)).ToList();
        zones[0].Unlock(); // Unlock the first zone by default
        GenerateGrid();

        //ONLY UNCOMMENT FOR TESTING PURPOSES
        resourceManager.AddCoins(9999);
    }

    private bool TryUnlockTile(Vector2Int position)
    {
        if (!IsUnlockedAt(position))
        {
            Debug.Log("Zone not unlocked");
            return false;
        }

        if (!IsAdjacentToUnlocked(position))
        {
            Debug.Log("Not adjacent to an unlocked tile");
            return false;
        }

        if (!HasEnoughCoins(position))
        {
            Debug.Log("Not enough coins");
            return false;
        }

        resourceManager.TrySpendCoins(unlockCost);

        return true;
    }
    private bool IsUnlockedAt(Vector2Int position)
    {
        foreach (ZoneRuntime zone in zones)
        {
            if (zone.IsPositionInZone(position))
                return zone.IsUnlocked;
        }

        Debug.Log("Zone has not been unlocked yet");
        return false;
    }

    private bool IsAdjacentToUnlocked(Vector2Int position)
    {
        Vector2Int[] adjacentPositions = new Vector2Int[]
        {
            new Vector2Int(position.x + 1, position.y),
            new Vector2Int(position.x - 1, position.y),
            new Vector2Int(position.x, position.y + 1),
            new Vector2Int(position.x, position.y - 1)
        };
        foreach (Vector2Int adjacent in adjacentPositions)
        {
            Tile adjacentTile = GetTileAt(adjacent);
            if (adjacentTile != null && adjacentTile.IsUnlocked)
                return true;
        }
        Debug.Log("No adjacent unlocked tiles found.");
        return false;
    }

    private bool HasEnoughCoins(Vector2Int position)
    {
        return resourceManager.Coins >= unlockCost;
    }


    private void GenerateGrid()
    {
        float xOffset = -(width * cellSize) / 2f + cellSize / 2f;
        float yOffset = -(height * cellSize) / 2f + cellSize / 2f - 0.03f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * cellSize + xOffset, y * cellSize + yOffset, 0f);
                Tile newTile = Instantiate(tilePrefab, position, Quaternion.identity, gridParent);

                // Store the tile in the grid
                grid[x, y] = newTile;

                Vector2Int gridPosition = new(x, y);

                FarmInfo info = GetFarmInfoAt(gridPosition);

                bool startUnlocked = (x == 0 && y == 0);

                newTile.InitializeCrop(info.cropData, gridPosition, startUnlocked, info.tileSprite);
                newTile.OnUnlockRequested += HandleUnlockRequested;
            }
        }
    }

    private bool IsInsideBounds(Vector2Int position)
    {
        if (position.x >= 0 && position.x < width && position.y >= 0 && position.y < height)
            return true;

        return false;
    }

    private Tile GetTileAt(Vector2Int position)
    {
        if (!IsInsideBounds(position))
        {
            Debug.Log($"Position {position} is out of bounds.");
            return null;
        }

        return grid[position.x, position.y];
    }

    private void HandleUnlockRequested(Tile tile)
    {
        bool success = TryUnlockTile(tile.GridPosition);

        if(success)
            tile.UnlockTile();
        if (!success)
            Debug.Log("Cannot unlock tile.");
    }

    private FarmInfo GetFarmInfoAt(Vector2Int position)
    {
        if (!IsInsideBounds(position))
        {
            Debug.Log("Position is out of bounds.");
            return null;
        }

        foreach (FarmInfo info in farmLayout.tiles)
        {
            if (info.position == position)
                return info;
        }

        Debug.LogError($"No CropData found at {position}");
        return null;
    }
}
