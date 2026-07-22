using UnityEngine;

[CreateAssetMenu(fileName = "Zone - ", menuName = "Farm/New Zone")]
public class ZoneData : ScriptableObject
{
    public string zoneName;
    public float zoneTier;

    public Vector2 minCorner;
    public Vector2 maxCorner;

    public bool IsPositionInZone(Vector2Int pos)
    {
        return pos.x >= minCorner.x && pos.x <= maxCorner.x
            && pos.y >= minCorner.y && pos.y <= maxCorner.y;
    }
}
