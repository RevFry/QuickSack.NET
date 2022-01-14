
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var source = new CancellationTokenSource();

var config = new ConfigurationBuilder()
    .AddEnvironmentVariables(prefix: "qs_")
    .Build();

var services = new ServiceCollection()
    .AddSingleton<FeedFactory>()
    .BuildServiceProvider();

var dToken = config["token"];
var token = source.Token;

var client = new DiscordClient(new DiscordConfiguration
{
    Token = dToken,
    TokenType = TokenType.Bot
});

var commands = client.UseCommandsNext(new CommandsNextConfiguration()
{
    StringPrefixes = new[] { "!qs", ">" },
    Services = services
});

commands.RegisterCommands<QSCommands>();

await client.ConnectAsync();

while (!token.IsCancellationRequested)
{
    await Task.Delay(100);
}

await client.DisconnectAsync();

