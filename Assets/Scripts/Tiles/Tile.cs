using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerClickHandler
{

    public event System.Action<Tile> OnHarvestRequested;
    public event System.Action<Tile> OnUnlockRequested;

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer soilRenderer;
    [SerializeField] private SpriteRenderer cropRenderer;
    [SerializeField] private SpriteRenderer fogRenderer;

    public CropData CropData {  get; private set; }

    public Vector2Int GridPosition { get; private set; }

    public float GrowthTimer {  get; private set; }

    public bool IsUnlocked { get; private set; }

    public bool IsAutomated { get; private set; }
    
    public bool IsMature { get; private set; }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            Interact();
    }

    public void InitializeCrop(CropData cropData, Vector2Int gridPosition, bool startUnlocked, Sprite groundSprite)
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

        soilRenderer.sprite = groundSprite;
        cropRenderer.sprite = cropData.cropSprite;

        UpdateFogVisibility();
    }

    private void UpdateFogVisibility()
    {
        fogRenderer.enabled = !IsUnlocked;
    }

    public void UnlockTile()
    {
        if(IsUnlocked)
        {
            Debug.LogWarning("Tile is already unlocked.");
            return;
        }

        IsUnlocked = true;
        UpdateFogVisibility();
    }

    public void Interact()
    {
        if(!IsUnlocked)
        {
            OnUnlockRequested?.Invoke(this);
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
