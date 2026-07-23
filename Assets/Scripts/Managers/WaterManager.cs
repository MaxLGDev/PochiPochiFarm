using System;
using UnityEngine;

public class WaterManager : MonoBehaviour
{
    public event Action<int> OnWaterChanged;

    [SerializeField] private ResourceManager resourceManager;

    public int Water { get; private set; } = 0;

    [SerializeField] private int maxWater;
    public int MaxWater => maxWater;

    [SerializeField] private int waterPrice;
    public int WaterPrice => waterPrice;

    [Header("Passive Water")]
    [SerializeField] private int passiveWaterRate = 1; // Amount of water gained per interval
    [SerializeField] private float passiveWaterInterval = 2.5f;
    public float PassiveWaterInterval => passiveWaterInterval;
    [SerializeField] private int waterPerClick = 1; // Amount of water gained per click
    private float regenTimer;

    private void Update()
    {
        RegenWater(passiveWaterInterval);
    }

    public void AddWater(int amount)
    {
        Water = Mathf.Clamp(Water + amount, 0, MaxWater);
        OnWaterChanged?.Invoke(Water);
        Debug.Log($"Added water. Total water: {Water}");
    }

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

    public void BuyWater()
    {
        if(resourceManager.Coins < WaterPrice)
        {
            Debug.Log($"Not enough coins to buy water. You need at least {WaterPrice} coins.");
            return;
        }

        if (Water >= MaxWater)
        {
            Debug.Log($"Cannot buy water. The water tank is already full.");
            return;
        }

        resourceManager.TrySpendCoins(WaterPrice);
        AddWater(waterPerClick);
    }

    private void RegenWater(float regenInterval)
    {
        if (Water < MaxWater)
        {
            regenTimer += Time.deltaTime;

            if (regenTimer >= regenInterval)
            {
                AddWater(passiveWaterRate);
                regenTimer = 0f;
            }
        }
    }


}
