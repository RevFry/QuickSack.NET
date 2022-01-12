namespace QuickSack.Net.Bot;

public static class QSExtensions
{
    private static QSBot Bot;

    public static DiscordClient AddQSBot(this DiscordClient client)
    {
        Bot = new QSBot();
        Bot.Initialize(client).Wait();
        return client;
    }

    public static DateTime StartOfWeek(this DateTime now, DayOfWeek startOfWeek)
    {
        int diff = (7 + (now.DayOfWeek - startOfWeek)) % 7;
        return now.AddDays(-1 * diff).Date;
    }
}
