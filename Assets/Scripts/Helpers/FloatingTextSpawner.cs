using UnityEngine;
using System;
using System.ComponentModel.Design.Serialization;
using UnityEngine.Localization;

/// <summary>
/// Spawns floating text in response to resource events.
/// Only listens and spawns, owns no game state.
/// </summary>
public class FloatingTextSpawner : MonoBehaviour
{
    // --- Localized Strings ---
    [SerializeField] private LocalizedString goldGainedText;

    [SerializeField] private LocalizedString goldSpentText;

    [SerializeField] private LocalizedString waterGainedText;

    [SerializeField] private LocalizedString waterSpentText;
    [SerializeField] private LocalizedString cropGainedText;
    [SerializeField] private LocalizedString cropNotResearchedtext;

    [SerializeField] private LocalizedString waterPurchaseFailedText;
    [SerializeField] private LocalizedString waterAlreadyFullText;
    [SerializeField] private LocalizedString notEnoughWaterText;
    
    // --- References ---
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private WaterManager waterManager;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private FloatingText floatingTextPrefab;
    
    // --- Spawn Points ---
    // Coin and water texts appear at these transforms (e.g. next to the HUD counters)
    [SerializeField] private Transform coinAnchor;
    [SerializeField] private Transform waterAnchor;
    [SerializeField] private Transform waterRefillAnchor;
    
    // Offset above a harvested tile so the text doesn't sit on the sprite.
    [SerializeField] private Vector3 tileOffset = new Vector3(0f, 0.5f, 0f);
    
    // --- Colors ---
    [SerializeField] private Color coinGainColor = new Color(1f, 0.85f, 0.1f);
    [SerializeField] private Color resourceSpentColor = Color.red;
    [SerializeField] private Color waterGainColor = new Color(0.3f, 0.7f, 1f);
    [SerializeField] private Color cropColor = new Color(0.4f, 0.9f, 0.3f);
    
    // --- Event Handlers
    private Action<int> onCoinsEarnedHandler;
    private Action<int> onCoinsSpentHandler;
    private Action<int> onWaterGainedHandler;
    private Action<int> onWaterSpentHandler;
    private Action<Tile, int> onCropHarvestHandler;
    private Action onWaterPurchaseFailedHandler;
    private Action onWaterAlreadyFullHandler;
    private Action onNotEnoughWaterHandler;
    private Action<Tile> onCropNotResearchedHandler;

    private void Awake()
    {
        onCoinsEarnedHandler = amount => SpawnLocalized(goldGainedText, amount, coinGainColor, coinAnchor.position);
        onCoinsSpentHandler  = amount => SpawnLocalized(goldSpentText, amount, resourceSpentColor, coinAnchor.position);
        onWaterGainedHandler = amount => SpawnLocalized(waterGainedText, amount, waterGainColor, waterAnchor.position);
        onWaterSpentHandler  = amount => SpawnLocalized(waterSpentText, amount, resourceSpentColor, waterAnchor.position);
        onWaterPurchaseFailedHandler = () => SpawnLocalized(waterPurchaseFailedText, resourceSpentColor, waterRefillAnchor.position);
        onWaterAlreadyFullHandler = () => SpawnLocalized(waterAlreadyFullText, waterGainColor, waterRefillAnchor.position);
        onNotEnoughWaterHandler = () => SpawnLocalized(notEnoughWaterText, resourceSpentColor, waterAnchor.position);
        onCropNotResearchedHandler = (tile) => SpawnLocalized(cropNotResearchedtext, resourceSpentColor, tile.transform.position + tileOffset);
        onCropHarvestHandler = SpawnCropGained;
    }

    private void OnEnable()
    {
        resourceManager.OnCoinsEarned += onCoinsEarnedHandler;
        resourceManager.OnCoinsSpent += onCoinsSpentHandler;
        resourceManager.OnCropHarvested += onCropHarvestHandler;
        waterManager.OnWaterGained += onWaterGainedHandler;
        waterManager.OnWaterSpent += onWaterSpentHandler;
        waterManager.OnWaterPurchaseFailed += onWaterPurchaseFailedHandler;
        waterManager.OnWaterAlreadyFull += onWaterAlreadyFullHandler;
        gridManager.OnCropNotResearched += onCropNotResearchedHandler;
        gridManager.OnNotEnoughWater += onNotEnoughWaterHandler;
    }

    private void OnDisable()
    {
        resourceManager.OnCoinsEarned -= onCoinsEarnedHandler;
        resourceManager.OnCoinsSpent -= onCoinsSpentHandler;
        resourceManager.OnCropHarvested -= onCropHarvestHandler;
        waterManager.OnWaterGained -= onWaterGainedHandler;
        waterManager.OnWaterSpent -= onWaterSpentHandler;
        waterManager.OnWaterPurchaseFailed -= onWaterPurchaseFailedHandler;
        waterManager.OnWaterAlreadyFull -= onWaterAlreadyFullHandler;
        gridManager.OnCropNotResearched -= onCropNotResearchedHandler;
        gridManager.OnNotEnoughWater -= onNotEnoughWaterHandler;
    }

    private void Spawn(string text, Color color, Vector3 position)
    {
        FloatingText instance = Instantiate(floatingTextPrefab, position, Quaternion.identity);
        instance.Setup(text, color);
    }

    private void SpawnLocalized(LocalizedString template, int amount, Color color, Vector3 position)
    {
        template.GetLocalizedStringAsync(amount).Completed += handle =>
        {
            Spawn(handle.Result, color, position);
        };
    }

    private void SpawnLocalized(LocalizedString template, Color color, Vector3 position)
    {
        template.GetLocalizedStringAsync().Completed += handle =>
        {
            Spawn(handle.Result, color, position);
        };
    }

    private void SpawnCropGained(Tile tile, int amount)
    {
        tile.CropData.LocalizedName.GetLocalizedStringAsync().Completed += nameHandle =>
        {
            cropGainedText.GetLocalizedStringAsync(amount, nameHandle.Result).Completed += textHandle =>
                Spawn(textHandle.Result, cropColor, tile.transform.position + tileOffset);
        };
    }
}
