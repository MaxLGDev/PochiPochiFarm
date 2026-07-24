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
            coinsText.text = $"<color=red>{newCoinCount}/{resourceManager.MaxCoins}</color>";
        }
        else if (newCoinCount == resourceManager.MaxCoins)
        {
            coinsText.text = $"<color=green>{newCoinCount}/{resourceManager.MaxCoins}</color>";
        }
        else
        {
            coinsText.text = $"<color=yellow>{newCoinCount}/{resourceManager.MaxCoins}</color>";
        }
    }
}