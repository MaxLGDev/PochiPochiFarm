using System;
using UnityEngine;
using TMPro;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private JournalManager journalManager;
    [SerializeField] private StatsManager statsManager;
    
    [Header("Journal Stats")]
    [SerializeField] private TMP_Text journalProgressionText;
    
    [Header("Upgrades Stats")]
    [SerializeField] private TMP_Text upgradesProgressionText;

    [Header("Playtime")]
    [SerializeField] private TMP_Text gameTotalPlaytimeText;

    private void Start()
    {
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

    private void HandleJournalProgression()
    {
        var (completed, total) = journalManager.GetTotalJournalProgress();
        float percent = total > 0 ? (float)completed / total * 100f : 0f;
        journalProgressionText.text = $"{completed}/{total}  ({percent:F0}%)";
    }

    private void UpdateTotalPlaytime() => gameTotalPlaytimeText.text = StatsFormatter.FormatPlaytime(statsManager.GetTotalPlaytime());
}
