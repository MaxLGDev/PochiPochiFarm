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
    
    public static string FormatNumber(int value)
    {
        if (value >= 1_000_000)
            return $"{value / 1_000_000f:0.#}M";

        if (value >= 1_000)
            return $"{value / 1_000f:0.#}K";

        return value.ToString();
    }
}