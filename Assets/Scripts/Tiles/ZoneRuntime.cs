using UnityEngine;

/// <summary>
/// Runtime instance of a zone.
/// Tracks whether the zone has been unlocked during gameplay.
/// </summary>
public class ZoneRuntime
{
    /// <summary>
    /// The static data that defines this zone.
    /// </summary>
    public ZoneData Data { get; }

    /// <summary>
    /// Indicates whether this zone has been unlocked.
    /// </summary>
    public bool IsUnlocked { get; private set; }

    public ZoneRuntime(ZoneData data) => Data = data;

    /// <summary>
    /// Returns whether the given position belongs to this zone.
    /// </summary>
    public bool IsPositionInZone(Vector2Int pos) => Data.IsPositionInZone(pos);

    /// <summary>
    /// Unlocks the zone.
    /// </summary>
    public void Unlock() => IsUnlocked = true;
}