using DSharpPlus.CommandsNext.Attributes;
using QuickSack.Shared;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Web;

namespace QuickSack.Net.Bot;

public class QSCommands : BaseCommandModule
{
    public FeedFactory feedFactory { private get; set; }

    [Command("search")]
    [Description("Search for existing show")]
    public async Task SearchCommand(CommandContext context, [RemainingText, Description("Text to query against the episode title and description")] string query)
    {
        await context.TriggerTypingAsync();

        List<FeedItem> results = await feedFactory.Search(query);
        
        StringBuilder sb = new StringBuilder();

        if (results.Count > 5) { results = results.Take(5).ToList(); }

        sb.AppendLine($"Top {results.Count} result(s)");

        results.ForEach(item =>
        {
            sb.AppendLine($"{item.Title} - https://quicksack.net/episode/{HttpUtility.UrlEncode(item.Title)}");
        });

        await context.RespondAsync(sb.ToString());
    }

    [Command("next")]
    [Description("Get the next episode planned for the show")]
    public async Task NextShowCommand(CommandContext context)
    {
        string NextEpisode = await feedFactory.GetNextEpisode();
        if (string.IsNullOrEmpty(NextEpisode)) { NextEpisode = "Unable to grab the next episode at the moment."; }
        await context.RespondAsync($"Looks like the next episdoe is {NextEpisode}");
    }

    [Command("last")]
    [Description("Get the last episode of the show")]
    public async Task LastShowCommand(CommandContext context)
    {
        await context.TriggerTypingAsync();

        var feed = await feedFactory.GetFeed();
        var item = feed.First();

        await context.RespondAsync($"{item.Title} - https://quicksack.net/episode/{HttpUtility.UrlEncode(item.Title)}");
    }

    [Command("details")]
    [Description("Get details for a given episode")]
    public async Task DetailsCommand(CommandContext context, [Description("episode number to get details for")] string episode)
    {
        var feed = await feedFactory.GetFeed();
        var item = feed.FirstOrDefault(x => x.Title.Contains(episode));

        string result = "I'm having trouble finding those details.";

        if(item != null) {
            result = $"{item.Title} - {item.PublishDate.ToShortDateString()}\n{item.Description}";
        }

        await context.RespondAsync(result);
    }

    [Hidden]
    [Command("dlx")]
    [Description("Replacement a host")]
    public async Task HostCommand(CommandContext context)
    {
        await context.TriggerTypingAsync();

        string textUrl = "https://uselessfacts.jsph.pl/random.json?language=en";
        string HostText = string.Empty;

        Stopwatch stopwatch = new Stopwatch();

        stopwatch.Start();

        using(HttpClient client = new HttpClient())
        {
            try
            {
                UselessHost UH = await client.GetFromJsonAsync<UselessHost>(textUrl);
                HostText = UH.text;
                stopwatch.Stop();
                var ToGenerate = stopwatch.Elapsed;
                
                StringBuilder sb = new StringBuilder();
                sb.Append("Did I ever tell you about the time I discovered... ");
                sb.AppendLine(HostText);
                sb.AppendLine("Useless fact fetched from https://uselessfacts.jsph.pl in ");
                sb.Append(ToGenerate.TotalMilliseconds.ToString());
                sb.AppendLine("ms with no insufferable ego required.");
                HostText = sb.ToString();
            }
            catch (Exception ex)
            {
                HostText = "I failed to find anything to say?";
            }
        }

        await context.RespondAsync(HostText);
    }

}
