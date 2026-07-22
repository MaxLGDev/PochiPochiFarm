using UnityEngine;

public class ZoneRuntime
{
    public ZoneData Data { get; }
    public bool IsUnlocked { get; private set; }

    public ZoneRuntime(ZoneData data) => Data = data;

    public bool IsPositionInZone(Vector2Int pos) => Data.IsPositionInZone(pos);
    public void Unlock() => IsUnlocked = true;
}