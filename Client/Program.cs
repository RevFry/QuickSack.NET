using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuickSack.Shared;
using QuickSack.Client.Code;
using Blazored.LocalStorage;
using Blazor.Analytics;

namespace QuickSack.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddBlazoredLocalStorage();

            using(HttpClient httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
            {
                GTag gTag = await httpClient.GetFromJsonAsync<GTag>("gtag.json");
                builder.Services.AddGoogleAnalytics(gTag.GoogleTag);
            }
            
            builder.Services.AddScoped<FeedRepository>();
            builder.Services.AddSingleton<SearchState>();
            builder.Services.AddSingleton<ToasterState>();

            await builder.Build().RunAsync();
        }
    }
}
