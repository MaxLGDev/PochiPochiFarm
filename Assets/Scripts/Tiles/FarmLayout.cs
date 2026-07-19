using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FarmInfo
{
    public Vector2Int position;
    public CropData cropData;
    public Sprite tileSprite;
}

[CreateAssetMenu(menuName = "Farm/Farm Layout")]
public class FarmLayout : ScriptableObject
{
    public List<FarmInfo> tiles;
}
