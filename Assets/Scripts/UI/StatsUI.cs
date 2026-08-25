using System;
using UnityEngine;
using TMPro;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private JournalManager journalManager;
    
    [Header("Journal Stats")]
    [SerializeField] private TMP_Text journalProgressionText;
    
    [Header("Upgrades Stats")]
    [SerializeField] private TMP_Text upgradesProgressionText;

    [Header("Playtime")]
    [SerializeField] private TMP_Text gameTotalPlaytimeText;

    private float totalPlaytime;

    private void Start()
    {
        totalPlaytime = PlayerPrefs.GetFloat("TotalPlaytime", 0f);
        HandleJournalProgression();
    }

    private void Update()
    {
        UpdateTotalPlaytime();
    }

    private void OnEnable()
    {
        journalManager.OnObjectiveClaimed += HandleJournalProgression;
        //upgradeManager.OnUpgradeBought += HandleUpgradesProgression;
    }

    private void OnDisable()
    {
        journalManager.OnObjectiveClaimed -= HandleJournalProgression;
    }

    private void UpdateDisplay()
    {
        
    }

    private void HandleJournalProgression()
    {
        var (completed, total) = journalManager.GetTotalJournalProgress();
        float percent = total > 0 ? (float)completed / total * 100f : 0f;
        journalProgressionText.text = $"{completed}/{total}  ({percent:F0}%)";
    }

    private void UpdateTotalPlaytime()
    {
        totalPlaytime += Time.deltaTime;
        gameTotalPlaytimeText.text = FormatPlaytime(totalPlaytime);
    }

    private string FormatPlaytime(float seconds)
    {
        int totalSeconds = (int)seconds;
        int minutes = (totalSeconds % 3600) / 60;
        int hours = totalSeconds / 3600;
        int secs = totalSeconds % 60;

        return $"{hours}h{minutes}m{secs}s";
    }
}
