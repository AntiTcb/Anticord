using Angler.Data;
using Angler.Features.RSS;
using Bots.Core;

namespace Angler;

public class Angler : DiscordBotBase
{
    protected override IHostBuilder CreateBotHostBuilder(string[]? args = null)
    {
        var builder = base.CreateBotHostBuilder(args);

        builder.ConfigureServices((_, services) =>
        {
            services.AddDbContextFactory<AnglerDbContext>();
            services.AddHostedService<YGOrgRssReader>();
        });

        return builder;
    }
}
