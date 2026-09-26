using UnityEngine;
using TMPro;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private JournalManager journalManager;
    [SerializeField] private UpgradeManager upgradeManager;
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
        HandleUpgradesProgression(null);
    }

    private void Update()
    {
        UpdateTotalPlaytime();
    }

    private void OnEnable()
    {
        journalManager.OnObjectiveClaimed += HandleJournalProgression;
        upgradeManager.OnUpgradeUnlocked += HandleUpgradesProgression;
    }

    private void OnDisable()
    {
        journalManager.OnObjectiveClaimed -= HandleJournalProgression;
        upgradeManager.OnUpgradeUnlocked -= HandleUpgradesProgression;
    }

    private void HandleJournalProgression()
    {
        var (completed, total) = journalManager.GetTotalJournalProgress();
        float percent = total > 0 ? (float)completed / total * 100f : 0f;
        journalProgressionText.text = $"{completed}/{total}  ({percent:F0}%)";
    }

    private void HandleUpgradesProgression(UpgradeData upgrade)
    {
        var (completed, total) = upgradeManager.GetTotalUpgradesProgress();
        float percent = total > 0 ? (float)completed / total * 100f : 0f;
        upgradesProgressionText.text = $"{completed}/{total} ({percent:F0}%)";
    }

    private void UpdateTotalPlaytime() => gameTotalPlaytimeText.text = StatsFormatter.FormatPlaytime(statsManager.GetTotalPlaytime());
}
