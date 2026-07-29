using UnityEngine;

public class SellCrops : MonoBehaviour
{
    private CropData cropData;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private Typewriter typewriter;

    public void Open(CropData crop)
    {
        cropData = crop;
        gameObject.SetActive(true);

        typewriter.ShowText($"Are you sure you want to sell all your <color=orange>{crop.CropName}</color>?");
    }

    public void Open()
    {
        gameObject.SetActive(true);
        typewriter.ShowText($"Are you sure you want to sell <color=orange> all your crops??</color>?");
    }

    public void Close()
    {
        gameObject.SetActive(false);
        cropData = null;
    }

    public void Sell()
    {
        if(cropData == null)
            resourceManager.SellAllCrops();
        else
        {
            resourceManager.TrySellCrops(cropData, resourceManager.GetCropCount(cropData));
            cropData = null;
        }

        Close();
    }
}
