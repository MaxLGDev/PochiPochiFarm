using UnityEngine;

[CreateAssetMenu(fileName = "New Objective", menuName = "Journal/Objective")]
public class ObjData : ScriptableObject
{
    // --- Objective ---
    public string Description;
    public int Target;
    public ObjectiveType Type;

    // Crop associated with this objective, if applicable.
    public CropData RelatedCrop;
}