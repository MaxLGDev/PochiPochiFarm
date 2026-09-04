using System;
using UnityEngine;

/// <summary>
/// Manages the player's water resource.
/// Handles spending, purchasing, and passive regeneration.
/// </summary>
public class WaterManager : MonoBehaviour
{
    // --- Events ---
    public event Action<int> OnWaterChanged;
    public event Action<int> OnWaterRefilled;
    private event Action<UpgradeData> onUpgradeUnlockedHandler;

    // --- References ---
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private UpgradeManager upgradeManager;

    // --- Water Settings ---
    [SerializeField] private int maxWater;
    [SerializeField] private int waterPrice;

    [Header("Passive Water")]
    [SerializeField] private int passiveWaterRate = 0;
    [SerializeField] private float passiveWaterInterval = 2.5f;

    // Water gained when purchasing water.
    [SerializeField] private int waterPerClick = 1;

    // --- State ---
    private float regenTimer;


    // ==============================
    // Properties
    // ==============================

    public int Water { get; private set; } = 0;
    public int MaxWater => maxWater;
    public int WaterPrice => waterPrice;
    public float PassiveWaterInterval => passiveWaterInterval;
    public float PassiveWaterRate => passiveWaterRate;


    // ==============================
    // Unity Lifecycle
    // ==============================

    private void Awake()
    {
        onUpgradeUnlockedHandler = HandleUpgradeUnlocked;
    }

    private void OnEnable()
    {
        upgradeManager.OnUpgradeUnlocked += onUpgradeUnlockedHandler;
    }

    private void OnDisable()
    {
        upgradeManager.OnUpgradeUnlocked -= onUpgradeUnlockedHandler;
    }

    private void Update()
    {
        RegenWater(passiveWaterInterval);
    }


    // ==============================
    // Water Management
    // ==============================

    /// <summary>
    /// Adds water up to the maximum capacity.
    /// </summary>
    public void AddWater(int amount)
    {
        Water = Mathf.Clamp(Water + amount, 0, MaxWater);

        OnWaterChanged?.Invoke(Water);
    }

    /// <summary>
    /// Spends water if enough is available.
    /// </summary>
    public void SpendWater(int amount)
    {
        if (amount > Water)
            return;

        Water -= amount;

        OnWaterChanged?.Invoke(Water);
    }

    /// <summary>
    /// Buys water using coins.
    /// </summary>
    public void BuyWater()
    {
        if (resourceManager.Coins < WaterPrice)
            return;

        if (Water >= MaxWater)
            return;

        resourceManager.TrySpendCoins(WaterPrice);

        AddWater(waterPerClick);
        OnWaterRefilled?.Invoke(waterPerClick);
    }

    private void HandleUpgradeUnlocked(UpgradeData upgrade)
    {
        switch (upgrade.EffectType)
        {
            case EffectType.MaxWater:
                maxWater += upgrade.EffectAmount;
                OnWaterChanged?.Invoke(Water);
                break;
            case EffectType.WaterRegen:
                passiveWaterRate += upgrade.EffectAmount;
                OnWaterRefilled?.Invoke(passiveWaterRate);
                break;
            default:
                break;
        }
    }

    // ==============================
    // Passive Regeneration
    // ==============================

    /// <summary>
    /// Regenerates water automatically over time.
    /// </summary>
    private void RegenWater(float regenInterval)
    {
        if (Water >= MaxWater)
            return;

        regenTimer += Time.deltaTime;

        if (regenTimer >= regenInterval)
        {
            AddWater(passiveWaterRate);
            regenTimer = 0f;
        }
    }
}