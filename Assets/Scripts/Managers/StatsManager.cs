using UnityEngine;

public class StatsManager : MonoBehaviour
{
     [SerializeField] private GridManager gridManager;
     [SerializeField] private ResourceManager resourceManager;
     [SerializeField] private JournalManager journalManager;
     
     private float totalPlaytime;
     private int totalGoldMade;
     private int totalCropsGathered;

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

     private void HandleCropGatheredCounter() => totalCropsGathered++;

     private void HandleCoinGatheredCounter(int obj) => totalGoldMade += obj;

     public void UpdateTotalPlaytime()
     {
          totalPlaytime += Time.deltaTime;
     }

     public float GetTotalPlaytime() => totalPlaytime;
     public int GetTotalGoldMade() => totalGoldMade;
     public int GetTotalCropsGathered() => totalCropsGathered;
}
