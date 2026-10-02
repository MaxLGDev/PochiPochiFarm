using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public TilesSaveData tiles = new TilesSaveData();
    public ZonesSaveData zones = new ZonesSaveData();
    public ResourcesSaveData resources = new ResourcesSaveData();
    public WaterSaveData water = new WaterSaveData();
    public LabSaveData lab = new LabSaveData();
    public UpgradesSaveData upgrades = new UpgradesSaveData();
    public JournalSaveData journal = new JournalSaveData();
    public StatsSaveData stats = new StatsSaveData();
}

// ---------- Tiles ----------
[Serializable]
public class TileSaveEntry
{
    public int x;
    public int y;
    public bool isUnlocked;
    public bool isMature;
    public float growthTimer;
}

[Serializable]
public class TilesSaveData
{
    public List<TileSaveEntry> tiles = new List<TileSaveEntry>();
}

// ---------- Zones ----------
[Serializable]
public class ZoneSaveEntry
{
    public string zoneId;
    public bool isUnlocked;
}

[Serializable]
public class ZonesSaveData
{
    public List<ZoneSaveEntry> zones = new List<ZoneSaveEntry>();
}

// ---------- Resources ----------
[Serializable]
public class CropCountSaveEntry
{
    public string cropName;
    public int count;
}

[Serializable]
public class ResourcesSaveData
{
    public int coins;
    public List<CropCountSaveEntry> cropCounts = new List<CropCountSaveEntry>();
}

// ---------- Water ----------
[Serializable]
public class WaterSaveData
{
    public int currentWater;
}

// ---------- Laboratory ----------
[Serializable]
public class LabCropSaveEntry
{
    public string cropName;
    public bool isResearched;
    public bool isAutomated;
}

[Serializable]
public class LabSaveData
{
    public List<LabCropSaveEntry> crops = new List<LabCropSaveEntry>();

    // In-progress work (empty string = nothing active)
    public string activeResearchCrop;
    public float researchElapsed;
    public string activeAutomationCrop;
    public float automationElapsed;
}

// ---------- Upgrades ----------
[Serializable]
public class UpgradesSaveData
{
    public List<string> boughtUpgradeIds = new List<string>();
}

// ---------- Journal ----------
[Serializable]
public class ObjectiveSaveEntry
{
    public string objectiveId;
    public int progress;
    public bool isCompleted;
    public bool isClaimed;
}

[Serializable]
public class JournalSaveData
{
    public bool lastChapterClaimed;
    public int highestUnlockedChapter;
    public List<ObjectiveSaveEntry> objectives = new List<ObjectiveSaveEntry>();
}

// ---------- Stats ----------
[Serializable]
public class StatsSaveData
{
    public int totalEarnedCoins;
    public float totalPlaytime;
    public int totalCropsGathered;
}