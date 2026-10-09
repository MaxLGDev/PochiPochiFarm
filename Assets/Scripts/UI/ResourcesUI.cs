using UnityEngine;

/// <summary>
/// Updates the coin counter displayed in the UI.
/// </summary>
public class ResourcesUI : MonoBehaviour
{
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private TMPro.TextMeshProUGUI coinsText;

    private void Awake()
    {
        UpdateCoinsUI(resourceManager.Coins);
    }

    private void OnEnable()
    {
        resourceManager.OnCoinsChanged += UpdateCoinsUI;
    }

    private void OnDisable()
    {
        resourceManager.OnCoinsChanged -= UpdateCoinsUI;
    }

    //==========================================================================
    // UI
    //==========================================================================

    /// <summary>
    /// Refreshes the displayed coin count.
    /// </summary>
    private void UpdateCoinsUI(int newCoinCount)
    {
        if (newCoinCount == 0)
        {
            coinsText.text = $"<color=red>{StatsFormatter.FormatNumber(newCoinCount)}/{StatsFormatter.FormatNumber(resourceManager.MaxCoins)}</color>";
        }
        else if (newCoinCount == resourceManager.MaxCoins)
        {
            coinsText.text = $"<color=green>{StatsFormatter.FormatNumber(newCoinCount)}/{StatsFormatter.FormatNumber(resourceManager.MaxCoins)}</color>";
        }
        else
        {
            coinsText.text = $"<color=yellow>{StatsFormatter.FormatNumber(newCoinCount)}/{StatsFormatter.FormatNumber(resourceManager.MaxCoins)}</color>";
        }
    }
}