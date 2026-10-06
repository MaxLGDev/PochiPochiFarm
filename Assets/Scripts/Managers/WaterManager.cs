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
    public event Action<int> OnWaterGained;
    public event Action<int> OnWaterSpent;
    private event Action<UpgradeData> onUpgradeUnlockedHandler;

    // --- References ---
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private UpgradeManager upgradeManager;

    // --- Water Settings ---
    [SerializeField] private int maxWater;
    [SerializeField] private int waterPrice;

    [Header("Passive Water")]
    [SerializeField] private int waterRefillPower = 0;
    [SerializeField] private float waterRefillSpeed = 10f;

    // Water gained when purchasing water.
    [SerializeField] private int waterPerClick = 1;

    // --- State ---
    private float regenTimer;
    public float RegenTimer => regenTimer;


    // ==============================
    // Properties
    // ==============================

    public int Water { get; private set; } = 0;
    private float skipRate = 0;
    public int MaxWater => maxWater;
    public int WaterPrice => waterPrice;
    public float WaterRefillSpeed => waterRefillSpeed;
    public float WaterRefillPower => waterRefillPower;


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
        RegenWater();
    }


    // ==============================
    // Water Management
    // ==============================

    /// <summary>
    /// Adds water up to the maximum capacity.
    /// </summary>
    public void AddWater(int amount)
    {
        int newWater = Mathf.Clamp(Water + amount, 0, MaxWater);
        int gained = newWater - Water;

        Water = newWater;

        if (gained > 0)
            OnWaterGained?.Invoke(gained);

        OnWaterChanged?.Invoke(Water);
    }

    /// <summary>
    /// Spends water if enough is available.
    /// </summary>
    public void SpendWater(int amount)
    {
        bool isWaterSkipped = false;

        float skipCheck = UnityEngine.Random.Range(0f, 100f);
        isWaterSkipped = skipCheck <= skipRate;

        if (isWaterSkipped)
            return;
        
        if (amount > Water)
            return;

        Water -= amount;

        OnWaterSpent?.Invoke(amount);
        OnWaterChanged?.Invoke(Water);
    }

    /// <summary>
    /// Buys water using coins.
    /// </summary>
    public void BuyWater()
    {
        if (resourceManager.Coins < WaterPrice)
        {
            SoundManager.Instance.PlaySFX("Blocked");
            return;
        }

        if (Water >= MaxWater)
        {
            SoundManager.Instance.PlaySFX("Blocked");
            return;
        }

        resourceManager.TrySpendCoins(WaterPrice);

        int amount = waterPerClick + waterRefillPower;
        AddWater(amount);
        OnWaterRefilled?.Invoke(amount);
        SoundManager.Instance.PlaySFX("WaterRefill", UnityEngine.Random.Range(0.85f, 1.15f));
    }

    private void HandleUpgradeUnlocked(UpgradeData upgrade)
    {
        switch (upgrade.EffectType)
        {
            case EffectType.MaxWater:
                maxWater += upgrade.EffectAmount;
                OnWaterChanged?.Invoke(Water);
                break;
            case EffectType.WaterRegenPower:
                waterRefillPower += upgrade.EffectAmount;
                OnWaterRefilled?.Invoke(waterRefillPower);
                break;
            case EffectType.WaterRegenSpeed:
                waterRefillSpeed = Mathf.Max(0.1f, waterRefillSpeed - upgrade.EffectAmount);
                OnWaterRefilled?.Invoke(waterRefillPower);
                break;
            case EffectType.SkipWaterChance:
                skipRate += upgrade.EffectAmount;
                OnWaterChanged?.Invoke(Water);
                break;
        }
    }

    // ==============================
    // Passive Regeneration
    // ==============================

    /// <summary>
    /// Regenerates water automatically over time.
    /// </summary>
    private void RegenWater()
    {
        if (Water >= MaxWater)
            return;

        if (waterRefillPower <= 0 || waterRefillSpeed <= 0)
            return;

        regenTimer += Time.deltaTime;

        if (regenTimer >= waterRefillSpeed)
        {
            AddWater(waterRefillPower);
            regenTimer -= waterRefillSpeed;
        }
    }
    
    public float RegenProgress => waterRefillSpeed <= 0f ? 0f : RegenTimer / waterRefillSpeed;

    public WaterSaveData CaptureState()
    {
        WaterSaveData data = new WaterSaveData();
        data.currentWater = Water;

        return data;
    }

    public void ApplyState(WaterSaveData data)
    {
        Water = data.currentWater;
        OnWaterChanged?.Invoke(Water);
    }
}