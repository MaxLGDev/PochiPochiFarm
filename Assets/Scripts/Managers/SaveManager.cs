using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;

// ============================================
// Save Data
// ============================================

// One slice per owning manager. Each manager fills and reads only its own slice.
[Serializable]
public class ManagerSaveData
{
    public UpgradesSaveData upgrades;
    public ResourcesSaveData resources;
    public WaterSaveData water;
    public LabSaveData lab;
    public TilesSaveData tiles;
    public ZonesSaveData zones;
    public JournalSaveData journal;
    public StatsSaveData stats;
    public TutorialSaveData tutorial;
}

// ============================================
// Save Manager
// ============================================

// Runs its Start() before every other script's Start(), so loaded state is in place
// before UI scripts read it. All Awake()/OnEnable() calls have already happened by then,
// so every manager's dictionaries, tiles and event subscriptions already exist.
[DefaultExecutionOrder(-100)]
public class SaveManager : MonoBehaviour
{
    // --- References ---
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private WaterManager waterManager;
    [SerializeField] private LaboratoryManager labManager;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private JournalManager journalManager;
    [SerializeField] private StatsManager statsManager;
    [SerializeField] private TutorialManager tutorialManager;

    private const string FileName = "save.json";

    private string SavePath => Path.Combine(Application.persistentDataPath, FileName);
    
    // ==============================
    // Unity Lifecycle
    // ==============================
 
    private void Start()
    {
        LoadGame();
    }
 
    private void OnApplicationQuit()
    {
        SaveGame();
    }
 
 
    // ==============================
    // Save / Load
    // ==============================

    public void SaveGame()
    {
        ManagerSaveData data = new ManagerSaveData
        {
            upgrades = upgradeManager.CaptureState(),
            resources = resourceManager.CaptureState(),
            water = waterManager.CaptureState(),
            lab = labManager.CaptureState(),
            tiles = gridManager.CaptureTiles(),
            zones = gridManager.CaptureZones(),
            journal = journalManager.CaptureState(),
            stats = statsManager.CaptureState(),
            tutorial = tutorialManager.CaptureState()
        };

        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
        Debug.Log($"Game saved to {SavePath}");
    }

    public void LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("No save file found, starting a fresh game");
            return;
        }

        try
        {
            ManagerSaveData data = JsonUtility.FromJson<ManagerSaveData>(File.ReadAllText(SavePath));

            if (data == null)
                return;

            // Order matters: upgrades first, so max water / coin cap are correct
            // before the current water / coins are restored and clamped against them.
            // Journal and stats come last, so events fired while restoring the other
            // systems can't leave them with progress that isn't in the save.

            upgradeManager.ApplyState(data.upgrades);
            resourceManager.ApplyState(data.resources);
            waterManager.ApplyState(data.water);
            labManager.ApplyState(data.lab);
            gridManager.ApplyTiles(data.tiles);
            gridManager.ApplyZones(data.zones);
            journalManager.ApplyState(data.journal);
            statsManager.ApplyState(data.stats);
            tutorialManager.ApplyState(data.tutorial);

            Debug.Log("Game loaded");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load save file: {e}");
        }
    }

    public void ResetSave()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
