using OrgBot;
using Serilog;

try
{
    var host = new OrgBotDiscordBot().CreateDiscordBotHost(args);

    await host.RunAsync();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly.");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}
