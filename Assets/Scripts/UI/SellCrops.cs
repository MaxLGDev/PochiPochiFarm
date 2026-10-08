using UnityEngine;

public class SellCrops : MonoBehaviour
{
    private CropData cropData;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private Typewriter typewriter;
    [SerializeField] private GameObject sellHalfButton;

    public void Open(CropData crop)
    {
        SoundManager.Instance.PlaySFX("TogglePanel");
        cropData = crop;
        sellHalfButton.SetActive(true);
        gameObject.SetActive(true);

        typewriter.ShowText($"Are you sure you want to sell <color=orange>{crop.CropName}</color>?");
    }

    public void Open()
    {
        SoundManager.Instance.PlaySFX("TogglePanel");
        cropData = null;
        sellHalfButton.SetActive(false);
        gameObject.SetActive(true);
        
        typewriter.ShowText($"Are you sure you want to sell <color=orange> all your crops??</color>?");
    }

    public void Close()
    {
        SoundManager.Instance.PlaySFX("TogglePanel");
        gameObject.SetActive(false);
        cropData = null;
    }

    public void Sell()
    {
        SoundManager.Instance.PlaySFX("SellCrop");
        if (cropData == null)
            resourceManager.SellAllCrops();
        else
            resourceManager.TrySellCrops(cropData, resourceManager.GetCropCount(cropData));
        
        Close();
    }

    public void SellHalf()
    {
        if (cropData == null)
            return;

        int count = resourceManager.GetCropCount(cropData);
        
        SoundManager.Instance.PlaySFX("SellCrop");

        resourceManager.TrySellCrops(cropData, (count + 1) / 2);

        Close();
    }
}
