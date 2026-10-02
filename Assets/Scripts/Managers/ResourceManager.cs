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
    public event Action<int> OnCoinsSpent;
    public event Action OnCoinsFull;
    public event Action<CropData, int> OnCropChanged;
    public event Action<Tile, int> OnCropHarvested;
    
    private Action<UpgradeData> handleUpgradeUnlocked;

    [SerializeField] private List<CropData> allCrops;

    // --- Coin Resources ---
    public int Coins { get; private set; }

    [SerializeField] private int maxCoins = 20;

    private int clickPower = 1;
    public int ClickPower => clickPower;

    private int yieldPower = 1;
    public int YieldPower => yieldPower;

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
        handleUpgradeUnlocked = HandleUpgradeUnlocked;
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

    private void HandleUpgradeUnlocked(UpgradeData upgrade)
    {
        switch (upgrade.EffectType)
        {
            case EffectType.MaxCoins:
                maxCoins += upgrade.EffectAmount;
                OnCoinsChanged?.Invoke(Coins);
                break;
            case EffectType.ClickMultiplier:
                clickPower += upgrade.EffectAmount;
                break;
            case EffectType.YieldMultiplier:
                yieldPower += upgrade.EffectAmount;
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
        int newCoins = Mathf.Min(Coins + amount, maxCoins);
        int gained = newCoins - Coins;

        Coins = newCoins;

        if (gained > 0)
            OnCoinsEarned?.Invoke(gained);
        
        OnCoinsChanged?.Invoke(Coins);
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
        
        OnCoinsSpent?.Invoke(amount);
        OnCoinsChanged?.Invoke(Coins);

        return true;
    }


    // ==============================
    // Harvesting
    // ==============================

    /// <summary>
    /// Processes a harvested tile.
    /// </summary>
    public void HandleHarvest(Tile tile, int power)
    {
        if (!tile)
            return;

        AddCrop(tile.CropData, power);
        OnCropHarvested?.Invoke(tile, power);
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

        if (coinRoom <= 0 && GetCropCount(crop) > 0)
            OnCoinsFull?.Invoke();

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
        if (Coins >= maxCoins)
        {
            OnCoinsFull?.Invoke();
            return;
        }
        
        // Sell crops from lowest to highest coin yield.
        var sortedCrops = cropInventory.Keys
            .OrderBy(crop => crop.CoinYield);

        foreach (CropData crop in sortedCrops)
        {
            TrySellCrops(crop, GetCropCount(crop));

            if (Coins >= maxCoins)
            {
                OnCoinsFull?.Invoke();
                break;
            }
        }
    }

    public ResourcesSaveData CaptureState()
    {
        ResourcesSaveData data = new ResourcesSaveData();
        data.coins = Coins;

        foreach (KeyValuePair<CropData, int> pair in cropInventory)
        {
            data.cropCounts.Add(new CropCountSaveEntry
            {
                cropName = pair.Key.name,
                count = pair.Value
            });
        }

        return data;
    }

    public void ApplyState(ResourcesSaveData data)
    {
        Coins = data.coins;
        cropInventory.Clear();

        foreach (CropCountSaveEntry entry in data.cropCounts)
        {
            CropData crop = allCrops.Find(c => c.name == entry.cropName);
            if (crop == null)
            {
                Debug.LogWarning($"Save refers to unknown crop '{entry.cropName}'");
                continue;
            }

            cropInventory[crop] = entry.count;
            OnCropChanged?.Invoke(crop, entry.count);
        }

        OnCoinsChanged?.Invoke(Coins);
    }
}