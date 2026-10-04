using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Manages the farm grid, including tile creation,
/// harvesting, and unlocking.
/// </summary>
public class GridManager : MonoBehaviour
{
    // --- Events ---
    public event Action OnTileHarvested;
    public event Action OnCropGathered;
    public event Action<CropData> OnCropUnlocked;

    // --- References ---
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private LaboratoryManager labManager;
    [SerializeField] private WaterManager waterManager;
    [SerializeField] private FarmLayout farmLayout;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private List<ZoneData> zonesData;

    // --- Grid Settings ---
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float cellSize = 1f;

    // --- Runtime Data ---
    private Tile[,] grid;
    private List<ZoneRuntime> zones;


    // ==============================
    // Unity Lifecycle
    // ==============================

    private void Awake()
    {
        if (farmLayout == null)
        {
            Debug.LogError("FarmLayout is not assigned.");
            return;
        }

        grid = new Tile[width, height];

        // Create runtime copies of the configured zones.
        zones = zonesData.Select(z => new ZoneRuntime(z)).ToList();

        // Unlock the starting zone.
        zones[0].Unlock();

        GenerateGrid();
    }

    private void OnEnable()
    {
        labManager.OnCropAutomated += HandleCropAutomated;
    }

    private void OnDisable()
    {
        labManager.OnCropAutomated -= HandleCropAutomated;
    }


    // ==============================
    // Tile Unlocking
    // ==============================

    /// <summary>
    /// Attempts to unlock the selected tile.
    /// </summary>
    private bool TryUnlockTile(Tile tile)
    {
        if (!IsUnlockedAt(tile.GridPosition))
        {
            Debug.Log("Zone not unlocked.");
            tile.FogBlockedAnimation();
            return false;
        }

        if (!IsAdjacentToUnlocked(tile.GridPosition))
        {
            Debug.Log("Not adjacent to an unlocked tile.");
            tile.FogBlockedAnimation();
            return false;
        }

        if (!resourceManager.HasEnoughCoinsForTile(tile))
        {
            Debug.Log("Not enough coins.");
            tile.FogBlockedAnimation();
            return false;
        }

        resourceManager.TrySpendCoins(tile.CropData.UnlockCost);

        return true;
    }

    /// <summary>
    /// Returns whether the zone containing the position is unlocked.
    /// </summary>
    private bool IsUnlockedAt(Vector2Int position)
    {
        foreach (ZoneRuntime zone in zones)
        {
            if (zone.IsPositionInZone(position))
                return zone.IsUnlocked;
        }

        Debug.Log("Zone has not been unlocked yet.");
        return false;
    }

    /// <summary>
    /// Checks if the position is adjacent to an unlocked tile.
    /// </summary>
    private bool IsAdjacentToUnlocked(Vector2Int position)
    {
        Vector2Int[] adjacentPositions =
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

    public void UnlockZone(Chapter chapter)
    {
        Debug.Log("Zone called.");

        foreach (ZoneRuntime zone in zones)
        {
            if (zone.Data.unlockChapterName != chapter.chapterName)
                continue;

            zone.Unlock();
            Debug.Log($"Zone {zone.Data.zoneName} unlocked.");
            return;
        }

        Debug.Log($"No zone found for chapter {chapter.chapterName}.");
    }


    // ==============================
    // Grid Generation
    // ==============================

    /// <summary>
    /// Creates every tile in the farm grid.
    /// </summary>
    private void GenerateGrid()
    {
        float xOffset = -(width * cellSize) / 2f + cellSize / 2f;
        float yOffset = -(height * cellSize) / 2f + cellSize / 2f - 0.3f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(
                    x * cellSize + xOffset,
                    y * cellSize + yOffset,
                    0f
                );

                Tile newTile = Instantiate(
                    tilePrefab,
                    position,
                    Quaternion.identity,
                    gridParent
                );

                // Store the tile for quick lookup.
                grid[x, y] = newTile;

                Vector2Int gridPosition = new(x, y);
                FarmInfo info = GetFarmInfoAt(gridPosition);

                bool startUnlocked = x == 0 && y == 0;

                newTile.OnHarvestRequested +=
                    tile => HandleHarvestRequested(tile, true);

                newTile.OnUnlockRequested += HandleUnlockRequested;
                newTile.OnCropMatured += HandleCropMatured;

                newTile.InitializeCrop(
                    info.cropData,
                    gridPosition,
                    startUnlocked,
                    info.tileSprite
                );
            }
        }
    }


    // ==============================
    // Grid Helpers
    // ==============================

    /// <summary>
    /// Returns whether the position is inside the grid.
    /// </summary>
    private bool IsInsideBounds(Vector2Int position)
    {
        return position.x >= 0 &&
               position.x < width &&
               position.y >= 0 &&
               position.y < height;
    }

    /// <summary>
    /// Returns the tile at the given position.
    /// </summary>
    private Tile GetTileAt(Vector2Int position)
    {
        if (!IsInsideBounds(position))
        {
            Debug.Log($"Position {position} is out of bounds.");
            return null;
        }

        return grid[position.x, position.y];
    }

    /// <summary>
    /// Returns the farm data assigned to a grid position.
    /// </summary>
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

        Debug.LogError($"No CropData found at {position}.");
        return null;
    }

    public bool IsCropUnlocked(CropData crop)
    {
        foreach (Tile tile in grid)
        {
            if (tile.CropData == crop && tile.IsUnlocked)
                return true;
        }

        return false;
    }


    // ==============================
    // Event Handlers
    // ==============================

    /// <summary>
    /// Handles tile unlock requests.
    /// </summary>
    private void HandleUnlockRequested(Tile tile)
    {
        bool success = TryUnlockTile(tile);

        if (success)
        {
            OnCropUnlocked?.Invoke(tile.CropData);
            tile.UnlockTile();
            SoundManager.Instance.PlaySFX("TileUnlock");
        }
        else
        {
            Debug.Log("Cannot unlock tile.");
        }
    }

    /// <summary>
    /// Handles crop harvesting.
    /// </summary>
    private bool HandleHarvestRequested(Tile tile, bool isManual)
    {
        if (!labManager.IsCropResearched(tile.CropData))
        {
            tile.CropBlockAnimation();
            return false;
        }

        if (tile.CropData.RequiredWater > 0)
        {
            if (waterManager.Water < tile.CropData.RequiredWater)
            {
                Debug.Log("Not enough water to harvest.");
                if (isManual)
                    SoundManager.Instance.PlaySFX("Blocked");
                return false;
            }

            waterManager.SpendWater(tile.CropData.RequiredWater);
        }

        resourceManager.HandleHarvest(tile, isManual ? resourceManager.ClickPower : resourceManager.YieldPower);
        tile.ResetGrowth();

        OnCropGathered?.Invoke();

        if (isManual)
        {
            OnTileHarvested?.Invoke();
            SoundManager.Instance.PlaySFX("TileHarvest", UnityEngine.Random.Range(0.85f, 1.15f));
            
        }

        return true;
    }

    private void HandleCropMatured(Tile tile) {
        if (!labManager.IsCropAutomated(tile.CropData))
            return;

        HandleHarvestRequested(tile, false);
    }

    private void HandleCropAutomated(CropData crop)
    {
        foreach (Tile tile in grid)
        {
            if (tile.CropData != crop)
                continue;

            if (!tile.IsMature)
                continue;

            HandleHarvestRequested(tile, false);
        }
    }
    
    // ==============================
    // SAVE SYSTEM
    // ==============================

    public TilesSaveData CaptureTiles()
    {
        TilesSaveData data = new TilesSaveData();

        foreach (Tile tile in grid)
        {
            data.tiles.Add(new TileSaveEntry
            {
                x = tile.GridPosition.x,
                y = tile.GridPosition.y,
                isUnlocked = tile.IsUnlocked,
                isMature = tile.IsMature,
                growthTimer = tile.GrowthTimer
            });
        }

        return data;
    }

    public void ApplyTiles(TilesSaveData data)
    {
        foreach (TileSaveEntry entry in data.tiles)
        {
            Tile tile = GetTileAt(new Vector2Int(entry.x, entry.y));
            if (tile == null)
            {
                Debug.LogWarning($"Save refers to an unknown tile ({entry.x},{entry.y})");
                continue;
            }
            tile.Restore(entry.isUnlocked, entry.isMature, entry.growthTimer);
        }
    }

    public ZonesSaveData CaptureZones()
    {
        ZonesSaveData data = new ZonesSaveData();

        foreach (ZoneRuntime zone in zones)
        {
            data.zones.Add(new ZoneSaveEntry
            {
                zoneId = zone.Data.name,
                isUnlocked = zone.IsUnlocked
            });
        }

        return data;
    }

    public void ApplyZones(ZonesSaveData data)
    {
        foreach (ZoneSaveEntry entry in data.zones)
        {
            ZoneRuntime zone = zones.Find(z => z.Data.name == entry.zoneId);
            if (zone == null)
            {
                Debug.LogWarning($"Save refers to an unknown zone ({entry.zoneId})");
                continue;
            }

            if (entry.isUnlocked)
                zone.Unlock();
        }
    }
}