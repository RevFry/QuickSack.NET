namespace QuickSack.Client.Code;

public static class Extensions
{
    public static DateTime StartOfWeek(this DateTime now, DayOfWeek startOfWeek)
    {
        int diff = (7 + (now.DayOfWeek - startOfWeek)) % 7;
        return now.AddDays(-1 * diff).Date;
    }
}
