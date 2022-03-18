using Serilog;
using Bot = Angler.Angler;

try
{
    var host = new Bot().CreateDiscordBotHost(args);

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