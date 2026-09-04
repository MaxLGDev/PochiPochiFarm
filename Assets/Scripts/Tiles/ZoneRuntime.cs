using UnityEngine;

/// <summary>
/// Runtime instance of a zone.
/// Tracks whether the zone has been unlocked during gameplay.
/// </summary>
public class ZoneRuntime
{
    // --- References ---
    /// <summary>
    /// The static data that defines this zone.
    /// </summary>
    public ZoneData Data { get; }

    // --- State ---
    /// <summary>
    /// Indicates whether this zone has been unlocked.
    /// </summary>
    public bool IsUnlocked { get; private set; }


    // ==============================
    // Initialization
    // ==============================

    public ZoneRuntime(ZoneData data)
    {
        Data = data;
    }


    // ==============================
    // Zone Checks
    // ==============================

    /// <summary>
    /// Returns whether the given position belongs to this zone.
    /// </summary>
    public bool IsPositionInZone(Vector2Int position)
    {
        return Data.IsPositionInZone(position);
    }


    // ==============================
    // Zone State
    // ==============================

    /// <summary>
    /// Unlocks the zone.
    /// </summary>
    public void Unlock()
    {
        IsUnlocked = true;
    }
}