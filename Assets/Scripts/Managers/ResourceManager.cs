using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages the player's resources, including coins and harvested crops.
/// </summary>
public class ResourceManager : MonoBehaviour
{
    // --- Events ---
    public event Action<int> OnCoinsChanged;
    public event Action<int> OnCoinsEarned;
    public event Action<CropData, int> OnCropChanged;
    
    private Action<UpgradeData> handleUpgradeUnlocked;

    // --- Coin Resources ---
    public int Coins { get; private set; }

    [SerializeField] private int maxCoins = 20;

    public int MaxCoins => maxCoins;

    // --- Crop Inventory ---
    private Dictionary<CropData, int> cropInventory = new();
    
    // ==============================
    // References
    // ==============================

    [SerializeField] private UpgradeManager upgradeManager;
    
    // ==============================
    // Lifecycle
    // ==============================

    private void Awake()
    {
        handleUpgradeUnlocked = HandleUpgraadeUnlocked;
    }

    private void OnEnable()
    {
        upgradeManager.OnUpgradeUnlocked += handleUpgradeUnlocked;
    }

    private void OnDisable()
    {
        upgradeManager.OnUpgradeUnlocked -= handleUpgradeUnlocked;
    }

    // ==============================
    // Resource Checks
    // ==============================

    /// <summary>
    /// Returns whether the player has enough coins to unlock the tile.
    /// </summary>
    public bool HasEnoughCoinsForTile(Tile tile)
    {
        return HasEnoughCoins(tile.CropData.UnlockCost);
    }

    /// <summary>
    /// Returns whether the player has enough coins for the requested amount.
    /// </summary>
    public bool HasEnoughCoins(int amount)
    {
        return Coins >= amount;
    }

    public bool HasEnough(CostEntry entry)
    {
        switch (entry.type)
        {
            case ResourceType.Coin:
                return Coins >= entry.amount;

            case ResourceType.Crop:
                return GetCropCount(entry.crop) >= entry.amount;

            default:
                return false;
        }
    }

    public bool CanAfford(List<CostEntry> costs)
    {
        foreach (CostEntry entry in costs)
        {
            if (!HasEnough(entry))
                return false;
        }

        return true;
    }

    public bool SpendResources(List<CostEntry> costs)
    {
        if (!CanAfford(costs))
            return false;

        foreach (CostEntry entry in costs)
        {
            switch (entry.type)
            {
                case ResourceType.Coin:
                    TrySpendCoins(entry.amount);
                    break;

                case ResourceType.Crop:
                    RemoveCrop(entry.crop, entry.amount);
                    break;

                default:
                    break;
            }
        }

        return true;
    }

    private void HandleUpgraadeUnlocked(UpgradeData upgrade)
    {
        switch (upgrade.EffectType)
        {
            case EffectType.MaxCoins:
                maxCoins += upgrade.EffectAmount;
                OnCoinsChanged?.Invoke(Coins);
                break;
            default:
                break;
        }
    }

    // ==============================
    // Crop Inventory
    // ==============================

    /// <summary>
    /// Adds harvested crops to the inventory.
    /// </summary>
    public void AddCrop(CropData crop, int amount)
    {
        if (crop == null)
            return;

        if (cropInventory.ContainsKey(crop))
            cropInventory[crop] += amount;
        else
            cropInventory[crop] = amount;

        OnCropChanged?.Invoke(crop, cropInventory[crop]);
    }

    /// <summary>
    /// Removes crops from the inventory.
    /// </summary>
    public void RemoveCrop(CropData crop, int amount)
    {
        if (crop == null)
            return;

        if (cropInventory.TryGetValue(crop, out int currentCount))
        {
            int newCount = Mathf.Max(0, currentCount - amount);

            cropInventory[crop] = newCount;
            OnCropChanged?.Invoke(crop, newCount);

            Debug.Log(
                $"Removed {amount} {crop.name}(s). Total {crop.name}s: {newCount}"
            );
        }
        else
        {
            Debug.Log($"No {crop.name}s to remove.");
        }
    }

    /// <summary>
    /// Returns the number of harvested crops in the inventory.
    /// </summary>
    public int GetCropCount(CropData crop)
    {
        if (cropInventory.TryGetValue(crop, out int count))
            return count;

        return 0;
    }


    // ==============================
    // Coins
    // ==============================

    /// <summary>
    /// Adds coins up to the maximum capacity.
    /// </summary>
    public void AddCoins(int amount)
    {
        Coins = Mathf.Min(Coins + amount, maxCoins);

        Debug.Log($"Added {amount} coins. Total coins: {Coins}");

        OnCoinsChanged?.Invoke(Coins);
        OnCoinsEarned?.Invoke(Coins);
    }

    /// <summary>
    /// Attempts to spend coins.
    /// </summary>
    public bool TrySpendCoins(int amount)
    {
        if (amount > Coins)
        {
            Debug.Log("Not enough coins to perform this action");
            return false;
        }

        Coins -= amount;

        Debug.Log($"Removed {amount} coins. Total coins: {Coins}");

        OnCoinsChanged?.Invoke(Coins);

        return true;
    }


    // ==============================
    // Harvesting
    // ==============================

    /// <summary>
    /// Processes a harvested tile.
    /// </summary>
    public void HandleHarvest(Tile tile)
    {
        if (tile == null)
            return;

        AddCrop(tile.CropData, 1);
    }


    // ==============================
    // Selling
    // ==============================

    /// <summary>
    /// Attempts to sell the requested number of crops.
    /// Returns the number of crops successfully sold.
    /// </summary>
    public int TrySellCrops(CropData crop, int amountRequested)
    {
        if (crop == null || amountRequested <= 0)
            return 0;

        int coinRoom = maxCoins - Coins;

        if (crop.CoinYield <= 0)
        {
            Debug.Log(
                $"Cannot sell {crop.name}s because its coin yield is zero or negative."
            );

            return 0;
        }

        int maxCropsToSell = Mathf.CeilToInt(
            (float)coinRoom / crop.CoinYield
        );

        int maxCropsSold = Mathf.Min(
            maxCropsToSell,
            GetCropCount(crop),
            amountRequested
        );

        if (maxCropsSold <= 0)
        {
            Debug.Log(
                $"Cannot sell any {crop.name}s. Either the coin storage is full or no crops are available."
            );

            return 0;
        }

        RemoveCrop(crop, maxCropsSold);
        AddCoins(crop.CoinYield * maxCropsSold);

        return maxCropsSold;
    }

    public void SellAllCrops()
    {
        // Sell crops from lowest to highest coin yield.
        var sortedCrops = cropInventory.Keys
            .OrderBy(crop => crop.CoinYield);

        foreach (CropData crop in sortedCrops)
        {
            TrySellCrops(crop, GetCropCount(crop));
        }
    }
}