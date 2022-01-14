using Blazor.Analytics;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using QuickSack.Client.Code;
using QuickSack.Net.Client;
using QuickSack.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddGoogleAnalytics("G-B2T863D9PL");
builder.Services.AddScoped<FeedRepository>();
builder.Services.AddSingleton<SearchState>();
builder.Services.AddSingleton<ToasterState>();

await builder.Build().RunAsync();
