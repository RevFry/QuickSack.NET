using DSharpPlus.EventArgs;
using QuickSack.Shared;
using System.Text;
using System.Web;

namespace QuickSack.Net.Bot;

public class QSBot
{
    private FeedFactory feedFactory;
    public async Task Initialize(DiscordClient client)
    {
        feedFactory = new FeedFactory();
        await feedFactory.Initialize();

        client.MessageCreated += OnMessageCreated;
    }

    private async Task OnMessageCreated(DiscordClient client, MessageCreateEventArgs args)
    {
        if (args.Message.Content.ToLower().StartsWith("!qs"))
        {
            string[] MessageParts = args.Message.Content.ToLower().Substring(3).Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            string Command = MessageParts.Length > 0 ? MessageParts[0] : "";

            switch (Command)
            {
                case "hello":
                    OnHelloMessage(client, args, MessageParts);
                    break;
                case "hi":
                    OnHelloMessage(client, args, MessageParts);
                    break;
                case "find":
                    await OnSearch(client, args, MessageParts);
                    break;
                case "search":
                    await OnSearch(client, args, MessageParts);
                    break;
                case "next":
                    await OnNextRequst(client, args, MessageParts);
                    break;
                case "details":
                    await OnDetailRequest(client, args, MessageParts);
                    break;
                case "help":
                    OnHelp(client, args, MessageParts);
                    break;
                default:
                    await client.SendMessageAsync(args.Channel, "I don't get what you're talking about. Try asking for help?");
                    break;
            }
        }
    }

    private async void OnHelp(DiscordClient client, MessageCreateEventArgs args, string[] messageParts)
    {
        await client.SendMessageAsync(args.Channel, "Lol, right.");
    }

    private async Task OnDetailRequest(DiscordClient client, MessageCreateEventArgs args, string[] messageParts)
    {
        await client.SendMessageAsync(args.Channel, "Coming soon.");
    }

    private async Task OnNextRequst(DiscordClient client, MessageCreateEventArgs args, string[] messageParts)
    {
        string NextEpisode = await feedFactory.GetNextEpisode();
        if (string.IsNullOrEmpty(NextEpisode)) { NextEpisode = "Unable to grab the next episode at the moment."; }
        await client.SendMessageAsync(args.Channel, $"Looks like the next episdoe is {NextEpisode}");
    }

    private async Task OnSearch(DiscordClient client, MessageCreateEventArgs args, string[] messageParts)
    {
        List<FeedItem> results = await feedFactory.Search(messageParts[1]);

        if(results.Count > 5) { results = results.Take(5).ToList(); }

        StringBuilder sb = new StringBuilder();

        results.ForEach(item =>
        {
            sb.AppendLine($"{item.Title} - https://quicksack.net/episode/{HttpUtility.UrlEncode(item.Title)}");
        });

        await client.SendMessageAsync(args.Channel, sb.ToString());
    }

    private async void OnHelloMessage(DiscordClient client, MessageCreateEventArgs args, string[] messageParts)
    {
        await client.SendMessageAsync(args.Channel, $"Uhh.. hello, {args.Author.Username}");
    }
}
