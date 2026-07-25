using UnityEngine;
using TMPro;

public class CropBox : MonoBehaviour
{
    [SerializeField] private CropData cropData;
    [SerializeField] private SellCrops sellCropsPanel;
    [SerializeField] private TextMeshProUGUI cropsQuantity;
    
    public void OpenSellPanel()
    {
        if (sellCropsPanel != null)
        {
            sellCropsPanel.Open(cropData);
        }
    }
}
