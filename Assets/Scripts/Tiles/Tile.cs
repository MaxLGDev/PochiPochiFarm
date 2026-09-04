using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Represents a single farm tile.
/// Handles crop growth, unlocking, harvesting, and player interaction.
/// </summary>
public class Tile : MonoBehaviour, IPointerClickHandler
{
    // --- Events ---
    public event System.Action<Tile> OnHarvestRequested;
    public event System.Action<Tile> OnUnlockRequested;
    public event System.Action<Tile> OnCropMatured;

    // --- Animation References ---
    [SerializeField] private PunchAnim punchAnim;
    [SerializeField] private WiggleAnim cropWiggle;
    [SerializeField] private WiggleAnim fogWiggle;
    [SerializeField] private PunchAnim fogPunch;

    // --- Sprite References ---
    [SerializeField] private SpriteRenderer soilRenderer;
    [SerializeField] private SpriteRenderer cropRenderer;
    [SerializeField] private SpriteRenderer fogRenderer;

    // --- State ---
    public CropData CropData { get; private set; }
    public Vector2Int GridPosition { get; private set; }
    public float GrowthTimer { get; private set; }
    public bool IsUnlocked { get; private set; }
    public bool IsAutomated { get; private set; }
    public bool IsMature { get; private set; }

    // Prevents updating the crop sprite every frame.
    private int lastStageIndex = -1;


    // ==============================
    // Unity Lifecycle
    // ==============================

    private void Update()
    {
        GrowCrop();
    }


    // ==============================
    // Crop Growth
    // ==============================

    /// <summary>
    /// Advances crop growth over time.
    /// </summary>
    private void GrowCrop()
    {
        if (!IsUnlocked)
            return;

        if (IsMature)
            return;

        GrowthTimer += Time.deltaTime;

        float fraction = GrowthTimer / CropData.GrowthTime;

        int rawIndex = Mathf.FloorToInt(
            fraction * (CropData.GrowthSprites.Length - 1)
        );

        int stageIndex = Mathf.Min(
            rawIndex,
            CropData.GrowthSprites.Length - 1
        );

        // Only update the sprite when the growth stage changes.
        if (stageIndex != lastStageIndex)
        {
            cropRenderer.sprite = CropData.GrowthSprites[stageIndex];
            lastStageIndex = stageIndex;
        }

        if (GrowthTimer >= CropData.GrowthTime)
        {
            IsMature = true;
            OnCropMatured?.Invoke(this);

            punchAnim.PunchScale();
            StartCoroutine(BlinkCropColor(0.3f));
        }
    }

    /// <summary>
    /// Briefly flashes the crop to indicate it has matured.
    /// </summary>
    private IEnumerator BlinkCropColor(float duration)
    {
        float timer = 0f;

        Color originalColor = cropRenderer.color;
        Color matureColorBlink = Color.yellow;

        while (timer < duration)
        {
            timer += 0.05f;

            cropRenderer.color = matureColorBlink;
            yield return new WaitForSeconds(0.05f);

            cropRenderer.color = originalColor;
        }

        cropRenderer.color = originalColor;
    }


    // ==============================
    // Player Interaction
    // ==============================

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            Interact();
    }

    /// <summary>
    /// Handles player interaction with the tile.
    /// </summary>
    public void Interact()
    {
        if (!IsUnlocked)
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

    public void CropBlockAnimation()
    {
        cropWiggle.Wiggle();
    }

    public void FogBlockedAnimation()
    {
        fogWiggle.Wiggle();
        fogPunch.PunchScale();
    }


    // ==============================
    // Initialization
    // ==============================

    /// <summary>
    /// Sets up the tile with its crop, position, and initial state.
    /// </summary>
    public void InitializeCrop(
        CropData cropData,
        Vector2Int gridPosition,
        bool startUnlocked,
        Sprite groundSprite
    )
    {
        if (cropData == null)
        {
            Debug.LogError("CropData is null. Cannot initialize crop.");
            return;
        }

        CropData = cropData;
        GridPosition = gridPosition;

        GrowthTimer = 0f;
        lastStageIndex = -1;

        IsUnlocked = startUnlocked;
        IsAutomated = false;

        if (cropData.GrowthTime <= 0f)
        {
            Debug.LogWarning(
                $"Crop {cropData.name} has a growth time of {cropData.GrowthTime}. " +
                "It will be considered mature immediately."
            );

            IsMature = true;
        }
        else
        {
            IsMature = false;
        }

        soilRenderer.sprite = groundSprite;

        // Display the mature sprite until growth begins.
        cropRenderer.sprite = CropData.GrowthSprites[
            CropData.GrowthSprites.Length - 1
        ];

        UpdateFogVisibility();
    }


    // ==============================
    // Tile State
    // ==============================

    /// <summary>
    /// Shows or hides the locked overlay.
    /// </summary>
    private void UpdateFogVisibility()
    {
        fogRenderer.enabled = !IsUnlocked;
    }

    /// <summary>
    /// Unlocks the tile.
    /// </summary>
    public void UnlockTile()
    {
        if (IsUnlocked)
        {
            Debug.LogWarning("Tile is already unlocked.");
            return;
        }

        IsUnlocked = true;
        UpdateFogVisibility();
    }

    /// <summary>
    /// Resets crop growth after harvesting.
    /// </summary>
    public void ResetGrowth()
    {
        if (CropData.GrowthTime > 0)
            IsMature = false;

        GrowthTimer = 0f;
        lastStageIndex = -1;
    }
}