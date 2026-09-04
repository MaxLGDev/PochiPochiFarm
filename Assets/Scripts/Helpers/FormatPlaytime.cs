public static class StatsFormatter
{
    // ==============================
    // Playtime
    // ==============================

    public static string FormatPlaytime(float seconds)
    {
        // Convert the total playtime into whole seconds.
        int totalSeconds = (int)seconds;

        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int remainingSeconds = totalSeconds % 60;

        return $"{hours}h{minutes}m{remainingSeconds}s";
    }
}