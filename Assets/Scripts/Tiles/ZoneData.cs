using UnityEngine;

/// <summary>
/// Defines the boundaries and information for a farm zone.
/// </summary>
[CreateAssetMenu(fileName = "Zone - ", menuName = "Farm/New Zone")]
public class ZoneData : ScriptableObject
{
    // --- Zone Information ---
    public string zoneName;
    public float zoneTier;
    public string unlockChapterName;

    // --- Bounds ---
    [Header("Bounds")]

    // Bottom-left corner of the zone.
    public Vector2 minCorner;

    // Top-right corner of the zone.
    public Vector2 maxCorner;


    // ==============================
    // Zone Checks
    // ==============================

    /// <summary>
    /// Returns whether the given grid position is inside this zone.
    /// </summary>
    public bool IsPositionInZone(Vector2Int position)
    {
        return position.x >= minCorner.x && position.x <= maxCorner.x
                                         && position.y >= minCorner.y && position.y <= maxCorner.y;
    }
}