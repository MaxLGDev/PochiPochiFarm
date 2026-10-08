using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameWonUI : MonoBehaviour
{
    [SerializeField] private StatsManager statsManager;
    [SerializeField] private JournalManager journalManager;

    [SerializeField] private GameObject gameWonPanel;
    
    [SerializeField] private TMP_Text gameTotalPlaytimeText;
    [SerializeField] private TMP_Text totalCropsGatheredText;
    [SerializeField] private TMP_Text totalGoldEarnedText;
    
    [SerializeField] private Button gameWonButton;
    [SerializeField] private TMP_Text closeGameText;
    [SerializeField] private float timerBeforeClose = 5f;

    private void OnEnable()
    {
        journalManager.OnLastChapterClaimed += HandleGameWon;
    }

    private void OnDisable()
    {
        journalManager.OnLastChapterClaimed -= HandleGameWon;
    }

    private void HandleGameWon()
    {
        gameWonPanel.SetActive(true);
        Time.timeScale = 0;

        gameTotalPlaytimeText.text = $"{StatsFormatter.FormatPlaytime(statsManager.GetTotalPlaytime())}";
        totalCropsGatheredText.text = $"{statsManager.GetTotalCropsGathered()}";
        totalGoldEarnedText.text = $"{statsManager.GetTotalGoldMade()}";
        //Add sounds

        gameWonButton.interactable = false;
        closeGameText.gameObject.SetActive(false);
        StartCoroutine(WaitBeforeCloseButtonAppears());
    }

    private IEnumerator WaitBeforeCloseButtonAppears()
    {
        yield return new WaitForSecondsRealtime(timerBeforeClose);
        gameWonButton.interactable = true;
        closeGameText.gameObject.SetActive(true);
    }
    
    
}
