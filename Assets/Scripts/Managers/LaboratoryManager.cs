using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Stores the research and automation state of a crop.
/// </summary>
public class LabState
{
    public bool IsResearched { get; private set; }
    public bool IsAutomated { get; private set; }

    public float ResearchTimer { get; private set; }
    public float AutomationTimer { get; private set; }

    /// <summary>
    /// Resets the research timer.
    /// </summary>
    public void StartResearch() => ResearchTimer = 0f;

    /// <summary>
    /// Advances the research timer.
    /// </summary>
    public void ProgressResearch() => ResearchTimer += Time.deltaTime;

    /// <summary>
    /// Resets the automation timer.
    /// </summary>
    public void StartAutomation() => AutomationTimer = 0f;

    /// <summary>
    /// Advances the automation timer.
    /// </summary>
    public void ProgressAutomation() => AutomationTimer += Time.deltaTime;

    /// <summary>
    /// Marks the crop as researched.
    /// </summary>
    public void FlagCropAsResearched() => IsResearched = true;

    /// <summary>
    /// Marks the crop as automated.
    /// </summary>
    public void FlagCropAsAutomated() => IsAutomated = true;
}

/// <summary>
/// Manages crop research and automation.
/// </summary>
public class LaboratoryManager : MonoBehaviour
{
    //==========================================================================
    // References
    //==========================================================================

    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private FarmLayout farmLayout;

    //==========================================================================
    // Research Data
    //==========================================================================

    [SerializeField] private List<CropData> researchableCrops;

    private readonly Dictionary<CropData, LabState> cropsResearch = new();

    private CropData currentResearchingCrop;
    private CropData currentAutomatingCrop;

    private void Start()
    {
        currentResearchingCrop = null;

        var allCrops = farmLayout.tiles.Select(t => t.cropData).Distinct();

        foreach (CropData crop in allCrops)
        {
            LabState state = new LabState();

            if (crop.startsResearched)
                state.FlagCropAsResearched();

            cropsResearch[crop] = state;
        }
    }

    private void Update()
    {
        UpdateResearchProgress();
        UpdateAutomationProgress();
    }

    public CropData GetCropAt(int index) => researchableCrops[index];

    //==========================================================================
    // Research
    //==========================================================================

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
    }

    /// <summary>
    /// Updates the active research progress.
    /// </summary>
    private void UpdateResearchProgress()
    {
        if (currentResearchingCrop == null)
            return;

        LabState state = cropsResearch[currentResearchingCrop];
        state.ProgressResearch();

        if (state.ResearchTimer >= currentResearchingCrop.ResearchDuration)
        {
            state.FlagCropAsResearched();
            currentResearchingCrop = null;
        }
    }

    /// <summary>
    /// Returns whether a research is currently in progress.
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

    //==========================================================================
    // Automation
    //==========================================================================

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
    }

    /// <summary>
    /// Updates the active automation progress.
    /// </summary>
    private void UpdateAutomationProgress()
    {
        if (currentAutomatingCrop == null)
            return;

        LabState state = cropsResearch[currentAutomatingCrop];
        state.ProgressAutomation();

        if (state.AutomationTimer >= currentAutomatingCrop.AutomationDuration)
        {
            state.FlagCropAsAutomated();
            currentAutomatingCrop = null;
        }
    }

    /// <summary>
    /// Returns whether an automation is currently in progress.
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
}