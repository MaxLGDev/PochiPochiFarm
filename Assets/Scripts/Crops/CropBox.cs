using UnityEngine;
using TMPro;

public class CropBox : MonoBehaviour
{
    // --- References ---
    [SerializeField] private CropData cropData;
    [SerializeField] private SellCrops sellCropsPanel;
    [SerializeField] private TextMeshProUGUI cropsCount;
    [SerializeField] private TextMeshProUGUI cropsName;
    [SerializeField] private Sprite cropIcon;
    [SerializeField] private ResourceManager resourceManager;


    // ==============================
    // Unity Lifecycle
    // ==============================

    private void Awake()
    {
        // Initialize the UI with the current crop data.
        UpdateCropUI(resourceManager.GetCropCount(cropData));
        cropsName.text = cropData.CropName;
    }

    private void OnEnable()
    {
        // Subscribe to crop count changes while this box is active.
        resourceManager.OnCropChanged += HandleCropChanged;
    }

    private void OnDisable()
    {
        // Always unsubscribe to prevent stale event subscriptions.
        resourceManager.OnCropChanged -= HandleCropChanged;
    }


    // ==============================
    // Public Methods
    // ==============================

    public void OpenSellPanel()
    {
        if (sellCropsPanel == null)
        {
            Debug.LogWarning($"{nameof(CropBox)}: Sell crops panel is not assigned.", this);
            return;
        }

        sellCropsPanel.gameObject.SetActive(true);
        sellCropsPanel.Open(cropData);
    }


    // ==============================
    // Event Handlers
    // ==============================

    private void HandleCropChanged(CropData crop, int newCount)
    {
        // Ignore changes belonging to other crop types.
        if (cropData != crop)
            return;

        UpdateCropUI(newCount);
    }


    // ==============================
    // UI
    // ==============================

    private void UpdateCropUI(int count)
    {
        cropsCount.text = count.ToString();
    }
}