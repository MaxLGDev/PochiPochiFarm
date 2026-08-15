using UnityEngine;

[CreateAssetMenu(fileName = "New Objective", menuName = "Journal/Objective")]
public class ObjData : ScriptableObject
{
    public string Description;
    public int Target;
    public ObjectiveType Type;
    public CropData RelatedCrop;
}
