
using Microsoft.Extensions.Configuration;

var source = new CancellationTokenSource();

var config = new ConfigurationBuilder()
    .AddEnvironmentVariables(prefix: "qs_")
    .Build();

var dToken = config["token"];

var client = new DiscordClient(new DiscordConfiguration
{
    Token = dToken,
    TokenType = TokenType.Bot
});

var token = source.Token;

await client.AddQSBot().ConnectAsync();

while (!token.IsCancellationRequested)
{
    await Task.Delay(100);
}

await client.DisconnectAsync();

