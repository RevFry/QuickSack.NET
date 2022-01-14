

using QuickSack.Shared;
using System.Net.Http.Json;

namespace QuickSack.Net.Bot;

public class FeedFactory
{
    private List<FeedItem> FeedItems { get; set; } = new List<FeedItem>();
    private DateTime FeedExpirationDate { get; set; }

    private string NextEpisode { get; set; } = string.Empty;
    private DateTime NextEpisodeExpirationDate { get; set;}

    private HttpClient client;

    public FeedFactory() {
        client = new HttpClient
        {
            BaseAddress = new Uri("https://quicksack.net/")
        };
    }

    public async Task Initialize()
    {
        await GetFeed();
        await GetNextEpisode();
    }

    public async Task<List<FeedItem>> Search(string query)
    {
        List<string> Tags = new List<string>(query.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        List<FeedItem> Items = new List<FeedItem>();

        List<FeedItem> Feeds = await GetFeed();

        if(Tags.Count > 0 && Feeds.Count > 0)
        {
            Items = Feeds.Where(x => Tags.Intersect(x.Tags).Count() == Tags.Count()).ToList();
        }

        return Items;
    }

    public async Task<List<FeedItem>> GetFeed()
    {
        if (FeedExpirationDate.CompareTo(DateTime.UtcNow) < 0 || FeedItems.Count == 0)
        {
            FeedItems = await client.GetFromJsonAsync<List<FeedItem>>("api/Feed");
            FeedExpirationDate = DateTime.UtcNow.StartOfWeek(DayOfWeek.Monday).AddDays(7);
        }
        return FeedItems;
    }

    public async Task<string> GetNextEpisode()
    {
        if (NextEpisodeExpirationDate.CompareTo(DateTime.UtcNow) < 0 || string.IsNullOrEmpty(NextEpisode))
        {
            NextEpisode = await client.GetStringAsync("api/next");
            NextEpisodeExpirationDate = DateTime.UtcNow.StartOfWeek(DayOfWeek.Monday).AddDays(1);
        }
        return NextEpisode;
    }

}
