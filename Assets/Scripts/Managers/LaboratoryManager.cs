using System.Collections.Generic;

using UnityEngine;

public class LabState
{
    public bool IsResearched { get; private set; }
    public bool IsAutomated { get; private set; }
    public float ResearchTimer { get; private set; }

    public void StartResearch() => ResearchTimer = 0f;
    public void ProgressResearch() => ResearchTimer += Time.deltaTime;

    public void FlagCropAsResearched() => IsResearched = true;
    public void FlagCropAsAutomated() => IsAutomated = true;
}

public class LaboratoryManager : MonoBehaviour
{
    private CropData currentCrop;
    [SerializeField] private ResourceManager resourceManager;

    [SerializeField] List<CropData> researchableCrops;
    Dictionary<CropData, LabState> cropsResearch = new Dictionary<CropData, LabState>();

    private void Start()
    {
        currentCrop = null;

        foreach (CropData crop in researchableCrops)
            cropsResearch[crop] = new LabState();
    }


    // Update is called once per frame
    void Update()
    {
        if (currentCrop == null)
            return;

        LabState state = cropsResearch[currentCrop];
        state.ProgressResearch();

        if (state.ResearchTimer >= currentCrop.ResearchDuration)
        {
            state.FlagCropAsResearched();
            currentCrop = null;
        }
    }

    public void StartResearching(CropData crop)
    {
        LabState state = cropsResearch[crop];

        if (state.IsResearched)
            return;

        if (currentCrop != null)
            return;

        if (!resourceManager.TrySpendCoins(crop.ResearchCost))
            return;

        currentCrop = crop;
        state.StartResearch();
    }


}
