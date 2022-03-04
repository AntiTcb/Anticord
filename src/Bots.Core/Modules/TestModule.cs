using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Discord;
using Discord.Interactions;
using Humanizer;
using Humanizer.Localisation;
using Microsoft.Extensions.Configuration;

namespace Bots.Core.Modules;

public class TestModule : InteractionModuleBase<ShardedInteractionContext>
{
    public IConfiguration Configuration { get; set; } = null!;

    [SlashCommand("echo", "Echos the input.")]
    public async Task Echo(string input) => await RespondAsync(input);

    [SlashCommand("info", "Information about the bot."), RequireBotPermission(ChannelPermission.EmbedLinks)]
    public async Task Info()
    {
        var app = await Context.Client.GetApplicationInfoAsync();
        int channelCount = Context.Client.Guilds.Sum(g => g.Channels.Count);
        int memberCount = Context.Client.Guilds.Sum(g => g.MemberCount);
        var assembly = Assembly.GetEntryAssembly()!.GetName();

        var eb = new EmbedBuilder
        {
            Author = new EmbedAuthorBuilder
            {
                Name = app.Owner.ToString(),
                IconUrl = app.Owner.GetAvatarUrl(),
                Url = "https://github.com/AntiTcb"
            },
            Description = "Check out the source code on GitHub!",
            Url = "https://github.com/AntiTcb/Anticord/", // TODO
            Title = $"{assembly.Name} {assembly.Version}"
        };
        eb.AddField("Library", $"[Discord.Net v{DiscordConfig.Version}](https://github.com/discord-net/Discord.Net)");
        eb.AddField("Runtime", $"{AppContext.TargetFrameworkName} {RuntimeInformation.ProcessArchitecture} " +
                $"({RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture})");
        eb.AddField("Uptime", GetUptime(), true);
        eb.AddField("Heap", GetHeapSize(), true);
        eb.AddField("Latency", Context.Client.Latency, true);
        eb.AddField("Guilds", Context.Client.Guilds.Count, true);
        eb.AddField("Channels", channelCount, true);
        eb.AddField("Users", memberCount, true);

        await ReplyAsync("", embed: eb.Build());
    }

    private static string GetUptime()=> (DateTimeOffset.UtcNow - Process.GetCurrentProcess().StartTime).Humanize(5, true, maxUnit: TimeUnit.Month);
    private static string GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).Megabytes().ToString();
}