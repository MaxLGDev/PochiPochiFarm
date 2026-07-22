using UnityEngine;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour
{
    [Header("Resources")]
    public int Coins { get; private set; }
    public int Water {  get; private set; }

    private Dictionary<CropData, int> cropInventory = new Dictionary<CropData, int>();

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

    public int GetCropCount(CropData crop)
    {
        if (cropInventory.TryGetValue(crop, out int count))
            return count;

        return 0;
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        Debug.Log($"Added {amount} coins. Total coins: {Coins}");
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
        return true;
    }
}
