using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// ============================================
// Laboratory State
// ============================================

/// <summary>
/// Stores the research and automation state of a crop.
/// </summary>
public class LabState
{
    // --- Research ---
    public bool IsResearched { get; private set; }
    public float ResearchTimer { get; private set; }

    // --- Automation ---
    public bool IsAutomated { get; private set; }
    public float AutomationTimer { get; private set; }


    // ==============================
    // Research
    // ==============================

    /// <summary>
    /// Resets the research timer.
    /// </summary>
    public void StartResearch()
    {
        ResearchTimer = 0f;
    }

    /// <summary>
    /// Advances the research timer.
    /// </summary>
    public void ProgressResearch(float speedMultiplier)
    {
        ResearchTimer += Time.deltaTime * speedMultiplier;
    }

    /// <summary>
    /// Marks the crop as researched.
    /// </summary>
    public void FlagCropAsResearched()
    {
        IsResearched = true;
    }


    // ==============================
    // Automation
    // ==============================

    /// <summary>
    /// Resets the automation timer.
    /// </summary>
    public void StartAutomation()
    {
        AutomationTimer = 0f;
    }

    /// <summary>
    /// Advances the automation timer.
    /// </summary>
    public void ProgressAutomation(float speedMultiplier)
    {
        AutomationTimer += Time.deltaTime * speedMultiplier;
    }

    /// <summary>
    /// Marks the crop as automated.
    /// </summary>
    public void FlagCropAsAutomated()
    {
        IsAutomated = true;
    }
    
    public void Restore(bool researched, bool automated, float researchTimer, float automationTimer)
    {
        IsResearched = researched;
        IsAutomated = automated;
        ResearchTimer = researchTimer;
        AutomationTimer = automationTimer;
    }
}


// ============================================
// Laboratory Manager
// ============================================

/// <summary>
/// Manages crop research and automation.
/// </summary>
public class LaboratoryManager : MonoBehaviour
{
    // --- Events ---
    public event Action<CropData> OnCropResearched;
    public event Action<CropData> OnCropAutomated;
    private Action<UpgradeData> onUpgradeUnlockedHandler;
    public event Action OnLabActionStarted;

    // --- References ---
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private FarmLayout farmLayout;
    [SerializeField] private JournalManager journalManager;

    // --- Research Data ---
    [SerializeField] private List<CropData> researchableCrops;

    // --- Runtime State ---
    private readonly Dictionary<CropData, LabState> cropsResearch = new();
    private float researchSpeedMultiplier = 1f;
    private float automationSpeedMultiplier = 1f;

    private CropData currentResearchingCrop;
    private CropData currentAutomatingCrop;


    // ==============================
    // Unity Lifecycle
    // ==============================

    private void Awake()
    {
        onUpgradeUnlockedHandler = HandleUpgradeUnlocked;
        
        var allCrops = farmLayout.tiles
            .Select(tile => tile.cropData)
            .Distinct();

        foreach (CropData crop in allCrops)
        {
            LabState state = new LabState();

            if (crop.StartsResearched)
                state.FlagCropAsResearched();

            cropsResearch[crop] = state;
        }
    }

    private void OnEnable()
    {
        upgradeManager.OnUpgradeUnlocked += onUpgradeUnlockedHandler;
    }

    private void OnDisable()
    {
        upgradeManager.OnUpgradeUnlocked -= onUpgradeUnlockedHandler;
    }

    private void Update()
    {
        UpdateResearchProgress();
        UpdateAutomationProgress();
    }

    private void HandleUpgradeUnlocked(UpgradeData upgrade)
    {
        switch (upgrade.EffectType)
        {
            case EffectType.ResearchSpeed:
                researchSpeedMultiplier += upgrade.EffectAmount;
                break;
            case EffectType.AutomationSpeed:
                automationSpeedMultiplier += upgrade.EffectAmount;
                break;
        }
    }

    // ==============================
    // Crop Access
    // ==============================

    public CropData GetCropAt(int index)
    {
        return researchableCrops[index];
    }


    // ==============================
    // Research
    // ==============================

    /// <summary>
    /// Starts researching the selected crop.
    /// </summary>
    public void StartResearching(CropData crop)
    {
        LabState state = cropsResearch[crop];

        if (state.IsResearched)
            return;

        if (currentResearchingCrop != null)
            return;

        if (!resourceManager.SpendResources(crop.ResearchCost))
            return;

        currentResearchingCrop = crop;
        state.StartResearch();
        OnLabActionStarted?.Invoke();
    }

    /// <summary>
    /// Updates the active research progress.
    /// </summary>
    private void UpdateResearchProgress()
    {
        if (currentResearchingCrop == null)
            return;

        LabState state = cropsResearch[currentResearchingCrop];
        state.ProgressResearch(researchSpeedMultiplier);

        if (state.ResearchTimer >= currentResearchingCrop.ResearchDuration)
        {
            CropData finished = currentResearchingCrop;
            
            state.FlagCropAsResearched();
            currentResearchingCrop = null;
            OnCropResearched?.Invoke(finished);
            SoundManager.Instance.PlaySFX("LabComplete");
        }
    }

    /// <summary>
    /// Returns whether research is currently in progress.
    /// </summary>
    public bool IsResearching()
    {
        return currentResearchingCrop != null;
    }

    /// <summary>
    /// Returns the current research progress as a value between 0 and 1.
    /// </summary>
    public float GetResearchProgress()
    {
        if (currentResearchingCrop == null)
            return 0f;

        LabState state = cropsResearch[currentResearchingCrop];

        return state.ResearchTimer / currentResearchingCrop.ResearchDuration;
    }

    public bool IsCropResearched(CropData crop)
    {
        if (crop == null)
            return false;

        if (!cropsResearch.ContainsKey(crop))
            return false;

        return cropsResearch[crop].IsResearched;
    }

    // ==============================
    // Automation
    // ==============================

    /// <summary>
    /// Starts automating the selected crop.
    /// </summary>
    public void StartAutomating(CropData crop)
    {
        LabState state = cropsResearch[crop];

        if (!state.IsResearched)
            return;

        if (state.IsAutomated)
            return;

        if (currentAutomatingCrop != null)
            return;

        if (!resourceManager.SpendResources(crop.AutomationCost))
            return;

        currentAutomatingCrop = crop;
        state.StartAutomation();
        OnLabActionStarted?.Invoke();
    }

    /// <summary>
    /// Updates the active automation progress.
    /// </summary>
    private void UpdateAutomationProgress()
    {
        if (currentAutomatingCrop == null)
            return;

        LabState state = cropsResearch[currentAutomatingCrop];
        state.ProgressAutomation(automationSpeedMultiplier);

        if (state.AutomationTimer >= currentAutomatingCrop.AutomationDuration)
        {
            CropData finished = currentAutomatingCrop;
            
            state.FlagCropAsAutomated();
            currentAutomatingCrop = null;
            OnCropAutomated?.Invoke(finished);
            SoundManager.Instance.PlaySFX("LabComplete");
        }
    }

    /// <summary>
    /// Returns whether automation is currently in progress.
    /// </summary>
    public bool IsAutomating()
    {
        return currentAutomatingCrop != null;
    }

    /// <summary>
    /// Returns the current automation progress as a value between 0 and 1.
    /// </summary>
    public float GetAutomationProgress()
    {
        if (currentAutomatingCrop == null)
            return 0f;

        LabState state = cropsResearch[currentAutomatingCrop];

        return state.AutomationTimer / currentAutomatingCrop.AutomationDuration;
    }

    public bool IsCropAutomated(CropData crop)
    {
        if (crop == null)
            return false;

        if (!cropsResearch.ContainsKey(crop))
            return false;

        return cropsResearch[crop].IsAutomated;
    }

    public LabSaveData CaptureState()
    {
        LabSaveData data = new LabSaveData();

        foreach (KeyValuePair<CropData, LabState> pair in cropsResearch)
        {
            data.crops.Add(new LabCropSaveEntry
            {
                cropName = pair.Key.name,
                isResearched = pair.Value.IsResearched,
                isAutomated = pair.Value.IsAutomated
            });
        }

        data.activeResearchCrop = "";
        if (currentResearchingCrop != null)
        {
            data.activeResearchCrop = currentResearchingCrop.name;
            data.researchElapsed = cropsResearch[currentResearchingCrop].ResearchTimer;
        }

        data.activeAutomationCrop = "";
        if (currentAutomatingCrop != null)
        {
            data.activeAutomationCrop = currentAutomatingCrop.name;
            data.automationElapsed = cropsResearch[currentAutomatingCrop].AutomationTimer;
        }

        return data;
    }

    public void ApplyState(LabSaveData data)
    {
        foreach (LabCropSaveEntry entry in data.crops)
        {
            CropData crop = FindCropByName(entry.cropName);
            if (crop == null)
            {
                Debug.LogWarning($"Save refers to unknown crop '{entry.cropName}'");
                continue;
            }

            float researchTimer = entry.cropName == data.activeResearchCrop ? data.researchElapsed : 0f;
            float automationTimer = entry.cropName == data.activeAutomationCrop ? data.automationElapsed : 0f;

            cropsResearch[crop].Restore(entry.isResearched, entry.isAutomated, researchTimer, automationTimer);
        }

        currentResearchingCrop =
                string.IsNullOrEmpty(data.activeResearchCrop) ? null : FindCropByName(data.activeResearchCrop);

        currentAutomatingCrop =
            string.IsNullOrEmpty(data.activeAutomationCrop) ? null : FindCropByName(data.activeAutomationCrop);
    }

    private CropData FindCropByName(string cropName)
    {
        return cropsResearch.Keys.FirstOrDefault(crop => crop.name == cropName);
    }

    /// <summary>
    /// True if the lab is idle for research or automation AND the player
    /// can pay for at least one crop that action could start on.
    /// </summary>
    public bool HasAvailableAction()
    {
        if (!IsResearching())
        {
            foreach (CropData crop in researchableCrops)
            {
                if (!cropsResearch[crop].IsResearched &&
                    resourceManager.CanAfford(crop.ResearchCost))
                {
                    return true;
                }
            }
        }

        if (!IsAutomating())
        {
            foreach (CropData crop in researchableCrops)
            {
                LabState state = cropsResearch[crop];

                if (state.IsResearched &&
                    !state.IsAutomated &&
                    resourceManager.CanAfford(crop.AutomationCost))
                {
                    return true;
                }
            }
        }

        return false;
    }
}