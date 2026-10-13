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

    private int lastShownSeconds = -1;

    private void Start()
    {
        HandleJournalProgression();
        HandleUpgradesProgression(null);
    }

    private void Update()
    {
        float playtime = statsManager.GetTotalPlaytime();
        int seconds = (int)playtime;

        if (seconds == lastShownSeconds)
            return;

        lastShownSeconds = seconds;
        gameTotalPlaytimeText.text = StatsFormatter.FormatPlaytime(playtime);
    }

    private void OnEnable()
    {
        journalManager.OnObjectiveClaimed += HandleJournalProgression;
        upgradeManager.OnUpgradeUnlocked += HandleUpgradesProgression;
        lastShownSeconds = -1;
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
}
