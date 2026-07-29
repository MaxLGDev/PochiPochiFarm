using UnityEngine;

public class SellAllButton : MonoBehaviour
{
    [SerializeField] private SellCrops sellCropsPanel;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private Typewriter typewriter;

    public void OpenSellPanel()
    {
        if (sellCropsPanel != null)
        {
            sellCropsPanel.Open();
        }
    }
}
