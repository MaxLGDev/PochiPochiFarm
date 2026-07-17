using UnityEngine;

public class Tile : MonoBehaviour
{

    public event System.Action<Tile> OnHarvestRequested;

    public CropData CropData {  get; private set; }

    public Vector2Int GridPosition { get; private set; }

    public float GrowthTimer {  get; private set; }

    public bool IsUnlocked { get; private set; }

    public bool IsAutomated { get; private set; }
    
    public bool IsMature { get; private set; }

    public void InitializeCrop(CropData cropData, Vector2Int gridPosition, bool startUnlocked)
    {
        if(cropData == null)
        {
            Debug.LogError("CropData is null. Cannot initialize crop.");
            return;
        }

        this.CropData = cropData;

        GridPosition = gridPosition;
        GrowthTimer = 0f;
        IsUnlocked = startUnlocked;
        IsAutomated = false;
        IsMature = false;
    }

    public void Interact()
    {
        if(!IsUnlocked)
        {
            Debug.Log("Tile is locked. Cannot interact.");
            return;
        }

        if (!IsMature)
        {
            Debug.Log("Tile isn't mature yet. Can't harvest.");
            return;
        }

        OnHarvestRequested?.Invoke(this);
    }
}
