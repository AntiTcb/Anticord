using System.Reflection;
using Bots.Core;
using OrgBot.Features.YugiohPrices.Api;
using OrgBot.Features.Yugipedia;
using RestEase.HttpClientFactory;
using WikiClientLibrary.Client;
using WikiClientLibrary.Sites;

namespace OrgBot;

public class OrgBot : DiscordBotBase
{
    protected override IHostBuilder CreateBotHostBuilder(string[]? args = null)
    {
        var builder = base.CreateBotHostBuilder(args);

        builder.ConfigureServices((_, services) =>
        {
            services.AddSingleton<YugipediaService>();
            services.AddSingleton(new WikiSite(new WikiClient { ClientUserAgent = $"OrgBot/{Assembly.GetEntryAssembly()!.GetName().Version} (AntiTcb#0001)" }, "https://yugipedia.com/api.php"));
            services.AddRestEaseClient<IYugiohPricesApi>();
        });

        return builder;
    }
}
