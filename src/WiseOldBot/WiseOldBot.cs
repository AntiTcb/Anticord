using Bots.Core;
using RestEase.HttpClientFactory;
using WiseOldBot.Features.GeTracker;
using WiseOldBot.Features.GeTracker.Api;

namespace WiseOldBot;

public class WiseOldBot : DiscordBotBase
{
    protected override IHostBuilder CreateBotHostBuilder(string[]? args = null)
    {
        var builder = base.CreateBotHostBuilder(args);

        builder.ConfigureServices((_, services) =>
        {
            services.AddHostedService<GeTrackerService>();
            services.AddSingleton<ItemMap>();
            services.AddRestEaseClient<IGeTrackerApi>();
        });

        return builder;
    }
}