public static class StatsFormatter
{
    public static string FormatPlaytime(float seconds)
    {
        int totalSeconds = (int)seconds;
        int minutes = (totalSeconds % 3600) / 60;
        int hours = totalSeconds / 3600;
        int secs = totalSeconds % 60;

        return $"{hours}h{minutes}m{secs}s";
    }
}