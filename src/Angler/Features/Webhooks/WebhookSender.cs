using Hangfire;
using Hangfire.PostgreSql;

namespace Angler.Features.Webhooks;

public class WebhookSender : BackgroundService
{
    public WebhookSender(IConfiguration configuration)
    {
        GlobalConfiguration.Configuration
            .UseRecommendedSerializerSettings()
            .UseSerilogLogProvider()
            .UsePostgreSqlStorage(configuration.GetConnectionString("Hangfire"));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var server = new BackgroundJobServer())
        {
            Console.WriteLine("Hangfire Server Started");
        }
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
