using System.Diagnostics;
using Bots.Core.Services;
using Discord;
using Discord.Addons.Hosting;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Bots.Core;

public class DiscordBotBase
{
    protected virtual IHostBuilder CreateBotHostBuilder(string[]? args = null)
    {
        // todo: json config
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
            .WriteTo.Console()
            .WriteTo.File(Path.Combine("./logs", $"{Process.GetCurrentProcess().ProcessName}.log"), restrictedToMinimumLevel: LogEventLevel.Error)
            .WriteTo.Sentry(o =>
            {
                o.Dsn = "https://662f6fe92b73451ebf81c0029d0e714d@o541633.ingest.sentry.io/5660782";
                o.Release = "1.1.0";
            })
            .CreateLogger();

        return Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .UseSystemd()
            .ConfigureServices((context, services) =>
            {
                services.AddHostedService<InteractionHandler>();
                //services.AddSingleton<ScriptService>();
                services.AddSingleton<Fergun.Interactive.InteractiveService>();
                services.AddInteractionService((config, _) =>
                {
                    config.LogLevel = LogSeverity.Verbose;
                    config.UseCompiledLambda = true;
                    config.DefaultRunMode = RunMode.Async;
                });
                services.AddDiscordShardedHost((config, __) =>
                {
                    config.SocketConfig = new()
                    {
                        LogLevel = LogSeverity.Verbose,
                        MessageCacheSize = 200,
                        GatewayIntents = GatewayIntents.AllUnprivileged,
                    };

                    config.Token = context.Configuration["Discord:Token"] ?? throw new ArgumentNullException(nameof(config.Token), "Discord Token is not configured.");
                    config.LogFormat = (message, exception) => exception is null ? $"{message.Source}: {message.Message}" : $"{message.Source}: [EXCEPTION] {exception}";
                });
            });
    }
    public IHost CreateDiscordBotHost(string[]? args = null)
    {
        return CreateBotHostBuilder(args).Build();
    }
}
