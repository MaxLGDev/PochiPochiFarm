using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "New Objective", menuName = "Journal/Objective")]
public class ObjData : ScriptableObject
{
    public LocalizedString localizedDescription;
    // --- Objective ---
    public string Description;
    public int Target;
    public ObjectiveType Type;

    // Crop associated with this objective, if applicable.
    public CropData RelatedCrop;
}