
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

var source = new CancellationTokenSource();

var config = new ConfigurationBuilder()
    .AddEnvironmentVariables(prefix: "qs_")
    .Build();

var logBuilder = new LoggerConfiguration()
    .WriteTo.Console();

if (!string.IsNullOrEmpty(config["SeqUrl"]))
{
    logBuilder = logBuilder.WriteTo.Seq(config["SeqUrl"], apiKey: config["SeqApiKey"]);
    if (!string.IsNullOrEmpty(config["SeqDebug"]))
    {
        Serilog.Debugging.SelfLog.Enable(Console.Error);
    }
}

Log.Logger = logBuilder.CreateLogger();

var logFactory = new LoggerFactory().AddSerilog();

var services = new ServiceCollection()
    .AddSingleton<IConfiguration>(config)
    .AddSingleton<FeedFactory>()
    .BuildServiceProvider();

var dToken = config["token"];
var token = source.Token;

var client = new DiscordClient(new DiscordConfiguration
{
    Token = dToken,
    TokenType = TokenType.Bot,
    LoggerFactory = logFactory
});

var commands = client.UseCommandsNext(new CommandsNextConfiguration()
{
    StringPrefixes = new[] { "!qs", ">" },
    Services = services
});

commands.RegisterCommands<QSCommands>();

client.Ready += Client_Ready;
client.ClientErrored += Client_Error;


await client.ConnectAsync();

while (!token.IsCancellationRequested)
{
    await Task.Delay(100);
}

await client.DisconnectAsync();

Task Client_Error(DiscordClient sender, ClientErrorEventArgs e)
{
    sender.Logger.LogError(e.Exception, "Error occured");
    return Task.CompletedTask;
}

Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
{
    sender.Logger.LogInformation("QuickSackBot is ready to go.");
    return Task.CompletedTask;
}
