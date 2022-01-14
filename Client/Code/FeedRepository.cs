using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using QuickSack.Shared;

namespace QuickSack.Client.Code;

public class FeedRepository
{
    public readonly ILocalStorageService localStorage;
    private readonly HttpClient client;

    private List<FeedItem> FeedItems { get; set; } = new List<FeedItem>();

    public FeedRepository(NavigationManager navigation, ILocalStorageService localStorageService)
    {
        client = new HttpClient { BaseAddress = new Uri(navigation.BaseUri) };
        localStorage = localStorageService;
    }
    public async Task<List<FeedItem>> GetFeedItems()
    {
        if (FeedItems.Count > 0)
        {
            return await Task.FromResult(FeedItems);
        }

        if (await localStorage.ContainKeyAsync("FeedExpireDate")
            && await localStorage.ContainKeyAsync("FeedItems")
            && (await localStorage.GetItemAsync<DateTime>("FeedExpireDate"))
                .CompareTo(DateTime.UtcNow) > 0)
        {
            FeedItems = await localStorage.GetItemAsync<List<FeedItem>>("FeedItems");
            return FeedItems;
        }
        else
        {
            FeedItems = await client.GetFromJsonAsync<List<FeedItem>>("api/Feed");
            DateTime CachExpires = DateTime.UtcNow.StartOfWeek(DayOfWeek.Monday).AddDays(7);
            await localStorage.SetItemAsync("FeedItems", FeedItems);
            await localStorage.SetItemAsync("FeedExpireDate", CachExpires);
            return FeedItems;
        }
    }

    public async Task<FeedItem> GetFeedItem(string Title)
    {
        if (FeedItems.Count == 0) { await GetFeedItems(); }

        FeedItem feedItem = FeedItems.FirstOrDefault(x => x.Title == Title);
        return await Task.FromResult(feedItem);
    }

    public async Task<string> GetNextEpisode()
    {
        if(await localStorage.ContainKeyAsync("NextExpireDate")
            && await localStorage.ContainKeyAsync("NextEpisode")
            && (await localStorage.GetItemAsync<DateTime>("NextExpireDate"))
                .CompareTo(DateTime.UtcNow) > 0)
        {
            return await localStorage.GetItemAsync<string>("NextEpisode");
        } else
        {
            string NextEpisode = await client.GetStringAsync("api/next");
            DateTime CacheExpires = DateTime.UtcNow.AddDays(1);
            await localStorage.SetItemAsync("NextEpisode", NextEpisode);
            await localStorage.SetItemAsync("NextExpireDate", CacheExpires);
            return NextEpisode;
        }
    }
}
