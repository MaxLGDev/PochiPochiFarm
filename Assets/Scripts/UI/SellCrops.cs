using UnityEngine;

public class SellCrops : MonoBehaviour
{
    private CropData cropData;
    [SerializeField] private ResourceManager resourceManager;

    public void Open(CropData crop)
    {
        cropData = crop;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        cropData = null;
    }

    public void Sell()
    {
        if (cropData == null)
        {
            Debug.LogError("Crop data is null.");
            return;
        }
       
        resourceManager.TrySellCrops(cropData, resourceManager.GetCropCount(cropData));
        cropData = null;
        Close();
    }
}
