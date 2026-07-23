using UnityEngine;

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

    private void UpdateCoinsUI(int newCoinCount)
    {
        coinsText.text = newCoinCount.ToString();

        // Update the UI to reflect the new water amount
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
