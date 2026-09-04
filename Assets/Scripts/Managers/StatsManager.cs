using UnityEngine;

public class StatsManager : MonoBehaviour
{
    // --- References ---
    [SerializeField] private GridManager gridManager;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private JournalManager journalManager;

    // --- Stats ---
    private float totalPlaytime;
    private int totalGoldMade;
    private int totalCropsGathered;


    // ==============================
    // Unity Lifecycle
    // ==============================

    private void Update()
    {
        UpdateTotalPlaytime();
    }

    private void OnEnable()
    {
        gridManager.OnCropGathered += HandleCropGatheredCounter;
        resourceManager.OnCoinsEarned += HandleCoinGatheredCounter;
    }

    private void OnDisable()
    {
        gridManager.OnCropGathered -= HandleCropGatheredCounter;
        resourceManager.OnCoinsEarned -= HandleCoinGatheredCounter;
    }


    // ==============================
    // Stats Tracking
    // ==============================

    private void HandleCropGatheredCounter()
    {
        totalCropsGathered++;
    }

    private void HandleCoinGatheredCounter(int amount)
    {
        totalGoldMade += amount;
    }

    public void UpdateTotalPlaytime()
    {
        totalPlaytime += Time.deltaTime;
    }


    // ==============================
    // Getters
    // ==============================

    public float GetTotalPlaytime()
    {
        return totalPlaytime;
    }

    public int GetTotalGoldMade()
    {
        return totalGoldMade;
    }

    public int GetTotalCropsGathered()
    {
        return totalCropsGathered;
    }
}