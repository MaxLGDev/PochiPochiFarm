using UnityEngine;
using TMPro;

public class CropBox : MonoBehaviour
{
    [SerializeField] private CropData cropData;
    [SerializeField] private SellCrops sellCropsPanel;
    [SerializeField] private TextMeshProUGUI cropsCount;
    [SerializeField] private TextMeshProUGUI cropsName;
    [SerializeField] private ResourceManager resourceManager;

    private void Awake()
    {
        cropsCount.text = resourceManager.GetCropCount(cropData).ToString();
        cropsName.text = cropData.CropName;
    }

    private void OnEnable()
    {
        resourceManager.OnCropChanged += HandleCropChanged;
    }

    private void OnDisable()
    {
        resourceManager.OnCropChanged -= HandleCropChanged;
    }

    public void OpenSellPanel()
    {
        if (sellCropsPanel != null)
        {
            sellCropsPanel.Open(cropData);
        }
    }

    private void HandleCropChanged(CropData crop, int newCount)
    {
        if (cropData != crop)
            return;

        cropsCount.text = newCount.ToString();
    }
}
