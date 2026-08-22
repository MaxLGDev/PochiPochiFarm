using UnityEngine;

/// <summary>
/// Defines the boundaries and information for a farm zone.
/// </summary>
[CreateAssetMenu(fileName = "Zone - ", menuName = "Farm/New Zone")]
public class ZoneData : ScriptableObject
{
    public string zoneName;
    public float zoneTier;
    public string unlockChapterName;

    [Header("Bounds")]

    // Bottom-left corner of the zone.
    public Vector2 minCorner;

    // Top-right corner of the zone.
    public Vector2 maxCorner;

    /// <summary>
    /// Returns whether the given grid position is inside this zone.
    /// </summary>
    public bool IsPositionInZone(Vector2Int pos)
    {
        return pos.x >= minCorner.x && pos.x <= maxCorner.x
            && pos.y >= minCorner.y && pos.y <= maxCorner.y;
    }
}