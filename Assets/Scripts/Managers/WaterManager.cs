using System;

using UnityEngine;

/// <summary>
/// Manages the player's water resource.
/// Handles spending, purchasing, and passive regeneration.
/// </summary>
public class WaterManager : MonoBehaviour
{
    //==========================================================================
    // Events
    //==========================================================================

    public event Action<int> OnWaterChanged;

    //==========================================================================
    // References
    //==========================================================================

    [SerializeField] private ResourceManager resourceManager;

    //==========================================================================
    // Properties
    //==========================================================================

    public int Water { get; private set; } = 0;

    [SerializeField] private int maxWater;
    public int MaxWater => maxWater;

    [SerializeField] private int waterPrice;
    public int WaterPrice => waterPrice;

    [Header("Passive Water")]

    // Water gained automatically every interval.
    [SerializeField] private int passiveWaterRate = 1;

    [SerializeField] private float passiveWaterInterval = 2.5f;
    public float PassiveWaterInterval => passiveWaterInterval;

    // Water gained when purchasing water.
    [SerializeField] private int waterPerClick = 1;

    private float regenTimer;

    private void Update()
    {
        RegenWater(passiveWaterInterval);
    }

    //==========================================================================
    // Water Management
    //==========================================================================

    /// <summary>
    /// Adds water up to the maximum capacity.
    /// </summary>
    public void AddWater(int amount)
    {
        Water = Mathf.Clamp(Water + amount, 0, MaxWater);

        OnWaterChanged?.Invoke(Water);
        Debug.Log($"Added water. Total water: {Water}");
    }

    /// <summary>
    /// Spends water if enough is available.
    /// </summary>
    public void SpendWater(int amount)
    {
        if (amount > Water)
        {
            Debug.Log($"Not enough water to spend. You have {Water} water.");
            return;
        }

        Water -= amount;

        OnWaterChanged?.Invoke(Water);
        Debug.Log($"Spent water. Total water: {Water}");
    }

    /// <summary>
    /// Buys water using coins.
    /// </summary>
    public void BuyWater()
    {
        if (resourceManager.Coins < WaterPrice)
        {
            Debug.Log($"Not enough coins to buy water. You need at least {WaterPrice} coins.");
            return;
        }

        if (Water >= MaxWater)
        {
            Debug.Log("Cannot buy water. The water tank is already full.");
            return;
        }

        resourceManager.TrySpendCoins(WaterPrice);
        AddWater(waterPerClick);
    }

    //==========================================================================
    // Passive Regeneration
    //==========================================================================

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