using UnityEngine;
using System.Collections.Generic;
using System;

public class ResourceManager : MonoBehaviour
{
    public event Action<int> OnCoinsChanged;

    [Header("Resources")]
    public int Coins { get; private set; }
    [SerializeField] private int maxCoins = 20;
    public int MaxCoins => maxCoins;

    private Dictionary<CropData, int> cropInventory = new Dictionary<CropData, int>();

    private void Awake()
    {
        AddCoins(20); // Start with 20 coins for testing
    }

    public void AddCrop(CropData crop, int amount)
    {
        if (crop == null)
            return;

        if (cropInventory.ContainsKey(crop))
            cropInventory[crop] += amount;
        else
            cropInventory[crop] = amount;

        Debug.Log($"Harvested! Total {crop.name}s: {cropInventory[crop]})");
    }

    public void RemoveCrop(CropData crop, int amount)
    {
        if (crop == null)
            return;
        if (cropInventory.TryGetValue(crop, out int currentCount))
        {
            int newCount = Mathf.Max(0, currentCount - amount);
            cropInventory[crop] = newCount;
            Debug.Log($"Removed {amount} {crop.name}(s). Total {crop.name}s: {newCount}");
        }
        else
        {
            Debug.Log($"No {crop.name}s to remove.");
        }
    }

    public int GetCropCount(CropData crop)
    {
        if (cropInventory.TryGetValue(crop, out int count))
            return count;

        return 0;
    }

    public void AddCoins(int amount)
    {
        Coins = Mathf.Min(Coins + amount, maxCoins);
        Debug.Log($"Added {amount} coins. Total coins: {Coins}");

        OnCoinsChanged?.Invoke(Coins);
    }

    public bool TrySpendCoins(int amount)
    {
        if (amount > Coins)
        {
            Debug.Log($"Not enough coins to perform this action");
            return false;
        }

        Coins -= amount;
        Debug.Log($"Removed {amount} coins. Total coins: {Coins}");
        OnCoinsChanged?.Invoke(Coins);
        return true;
    }

    public void HandleHarvest(Tile tile)
    {
        if (tile == null) return;

        AddCrop(tile.CropData, 1);
        Debug.Log($"Harvested {tile.CropData.name} from tile at {tile.GridPosition}. Total {tile.CropData.name}s: {GetCropCount(tile.CropData)}");
    }

    public void SellCrop(CropData crop, int amount)
    {
        if (crop == null || amount <= 0)
            return;

        int currentCount = GetCropCount(crop);
        if (currentCount < amount)
        {
            Debug.Log($"Not enough {crop.name}s to sell. Current count: {currentCount}");
            return;
        }
        RemoveCrop(crop, amount);
        AddCoins(Mathf.RoundToInt(crop.coinYield * amount));
        OnCoinsChanged?.Invoke(Coins);
    }
}
