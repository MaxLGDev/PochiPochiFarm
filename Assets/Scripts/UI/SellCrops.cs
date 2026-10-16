using UnityEngine;
using UnityEngine.Localization;

public class SellCrops : MonoBehaviour
{
    private CropData cropData;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private Typewriter typewriter;
    [SerializeField] private GameObject sellHalfButton;

    [SerializeField] private LocalizedString sellAllPrompt;
    [SerializeField] private LocalizedString sellCropPrompt;

    public void Open(CropData crop, string localizedName)
    {
        SoundManager.Instance.PlaySFX("TogglePanel");
        cropData = crop;
        sellHalfButton.SetActive(true);
        gameObject.SetActive(true);

        sellCropPrompt.Arguments = new object[] { localizedName };
        
        sellCropPrompt.GetLocalizedStringAsync().Completed += handle =>
            typewriter.ShowText(handle.Result);
    }

    public void Open()
    {
        SoundManager.Instance.PlaySFX("TogglePanel");
        cropData = null;
        sellHalfButton.SetActive(false);
        gameObject.SetActive(true);

        sellAllPrompt.GetLocalizedStringAsync().Completed +=
            handle => typewriter.ShowText(handle.Result);
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
