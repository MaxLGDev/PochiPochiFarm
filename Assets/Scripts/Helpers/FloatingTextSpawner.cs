using UnityEngine;
using System;
using UnityEngine.Serialization;

/// <summary>
/// Spawns floating text in response to resource events.
/// Only listens and spawns, owns no game state.
/// </summary>
public class FloatingTextSpawner : MonoBehaviour
{
    // --- References ---
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private WaterManager waterManager;
    [SerializeField] private FloatingText floatingTextPrefab;
    
    // --- Animations ---
    [SerializeField] private PunchAnim punchAnim;
    [SerializeField] private WiggleAnim wiggleAnim;
    
    // --- Spawn Points ---
    // Coin and water texts appear at these transforms (e.g. next to the HUD counters)
    [SerializeField] private Transform coinAnchor;
    [SerializeField] private Transform waterAnchor;
    
    // Offset above a harvested tile so the text doesn't sit on the sprite.
    [SerializeField] private Vector3 tileOffset = new Vector3(0f, 0.5f, 0f);
    
    // --- Colors ---
    [SerializeField] private Color coinGainColor = new Color(1f, 0.85f, 0.1f);
    [SerializeField] private Color coinSpentColor = Color.red;
    [SerializeField] private Color waterGainColor = new Color(0.3f, 0.7f, 1f);
    [SerializeField] private Color waterSpentColor = new Color(0.8f, 0.3f, 0.3f);
    [SerializeField] private Color cropColor = new Color(0.4f, 0.9f, 0.3f);
    
    // --- Event Handlers
    private Action<int> onCoinsEarnedHandler;
    private Action<int> onCoinsSpentHandler;
    private Action<int> onWaterGainedHandler;
    private Action<int> onWaterSpentHandler;
    private Action<Tile, int> onCropHarvestHandler;

    private void Awake()
    {
        onCoinsEarnedHandler = amount => Spawn($"+{amount} GOLD", coinGainColor, coinAnchor.position);
        onCoinsSpentHandler = amount => Spawn($"-{amount} GOLD", coinSpentColor, coinAnchor.position);
        onWaterGainedHandler = amount => Spawn($"+{amount} WATER", waterGainColor, waterAnchor.position);
        onWaterSpentHandler = amount => Spawn($"-{amount} WATER", waterSpentColor, waterAnchor.position);
        onCropHarvestHandler = (tile, amount) => Spawn($"+{amount} {tile.CropData.CropName.ToUpper()}", cropColor, tile.transform.position + tileOffset);
    }

    private void OnEnable()
    {
        resourceManager.OnCoinsEarned += onCoinsEarnedHandler;
        resourceManager.OnCoinsSpent += onCoinsSpentHandler;
        resourceManager.OnCropHarvested += onCropHarvestHandler;
        waterManager.OnWaterGained += onWaterGainedHandler;
        waterManager.OnWaterSpent += onWaterSpentHandler;
    }

    private void OnDisable()
    {
        resourceManager.OnCoinsEarned -= onCoinsEarnedHandler;
        resourceManager.OnCoinsSpent -= onCoinsSpentHandler;
        resourceManager.OnCropHarvested -= onCropHarvestHandler;
        waterManager.OnWaterGained -= onWaterGainedHandler;
        waterManager.OnWaterSpent -= onWaterSpentHandler;
    }

    private void Spawn(string text, Color color, Vector3 position)
    {
        FloatingText instance = Instantiate(floatingTextPrefab, position, Quaternion.identity);
        instance.Setup(text, color);
    }
}
