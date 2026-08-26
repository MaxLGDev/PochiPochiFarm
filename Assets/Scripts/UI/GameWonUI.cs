using System;
using UnityEngine;
using TMPro;

public class GameWonUI : MonoBehaviour
{
    [SerializeField] private StatsManager statsManager;
    [SerializeField] private JournalManager journalManager;

    [SerializeField] private GameObject gameWonPanel;
    
    [SerializeField] private TMP_Text gameTotalPlaytimeText;
    [SerializeField] private TMP_Text totalCropsGatheredText;
    [SerializeField] private TMP_Text totalGoldEarnedText;

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

        gameTotalPlaytimeText.text = $"Total playtime: {StatsFormatter.FormatPlaytime(statsManager.GetTotalPlaytime())}";
        totalCropsGatheredText.text = $"Total crops gathered: {statsManager.GetTotalCropsGathered()}";
        totalGoldEarnedText.text = $"Total gold earned: {statsManager.GetTotalGoldMade()}";
        //Add sounds
    }
}
