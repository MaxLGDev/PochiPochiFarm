using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private FarmLayout farmLayout;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform gridParent;

    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;

    [SerializeField] private float cellSize = 1f;

    private Tile[,] grid;

    private void Awake()
    {
        if(farmLayout == null)
        {
            Debug.LogError("FarmLayout is not assigned");
            return;
        }

        grid = new Tile[width, height];

        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * cellSize, y * cellSize, 0f);
                Tile newTile = Instantiate(tilePrefab, position, Quaternion.identity, gridParent);

                // Store the tile in the grid
                grid[x, y] = newTile;

                Vector2Int gridPosition = new(x, y);

                CropData crop = GetCropDataAt(gridPosition);

                bool startUnlocked = x == width / 2 && y == height / 2; // Unlock the center tile

                newTile.InitializeCrop(crop, gridPosition, startUnlocked);
            }
        }
    }

    private CropData GetCropDataAt(Vector2Int position)
    {
        foreach(FarmInfo tile in farmLayout.tiles)
        {
            if (tile.position == position)
                return tile.cropData;
        }

        Debug.LogError($"No CropData found at {position}");
        return null;
    }
}
