using System;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectiveType
{
    ClickCount,
    CoinsEarned,
    UnlockCrop,
    ResearchCrop,
    AutomateCrop,
    WaterRefilled,
    CropGathered
}

class ObjState
{
    public int Progress { get; private set; }
    public bool IsComplete { get; private set; }

    public void IncreaseProgress(int amount) => Progress += amount;
    public void SetObjectiveComplete() => IsComplete = true;
}

[Serializable]
public class Chapter
{
    public string chapterName;
    public List<ObjData> objectives = new();
}

public class JournalManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private LaboratoryManager labManager;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private WaterManager waterManager;

    [SerializeField] List<Chapter> chaptersList;

    Dictionary<ObjData, ObjState> objectivesStates = new();

    private void Awake()
    {
        foreach(Chapter chap in chaptersList)
        {
            foreach(ObjData obj in chap.objectives)
            {
                ObjState state = new ObjState();

                objectivesStates[obj] = state;

                
            }
        }

        Debug.Log(objectivesStates.Count);
    }


    private void OnEnable()
    {
        resourceManager.OnCoinsEarned += HandleCoinsEarned;
    }

    private void OnDisable()
    {
        resourceManager.OnCoinsEarned -= HandleCoinsEarned;
    }

    private void HandleCoinsEarned(int amount)
    {
        foreach(ObjData obj in objectivesStates.Keys)
        {
            if(obj.Type == ObjectiveType.CoinsEarned)
            {
                objectivesStates[obj].IncreaseProgress(amount);

                if (objectivesStates[obj].Progress >= obj.Target)
                    objectivesStates[obj].SetObjectiveComplete();
            }
        }
    }

}
