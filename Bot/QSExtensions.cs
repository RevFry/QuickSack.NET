namespace QuickSack.Net.Bot;

public static class QSExtensions
{
    public static DateTime StartOfWeek(this DateTime now, DayOfWeek startOfWeek)
    {
        int diff = (7 + (now.DayOfWeek - startOfWeek)) % 7;
        return now.AddDays(-1 * diff).Date;
    }
}
