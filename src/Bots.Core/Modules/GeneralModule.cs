using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Discord;
using Discord.Interactions;
using Humanizer;
using Humanizer.Localisation;
using Microsoft.Extensions.Configuration;

namespace Bots.Core.Modules;

public class GeneralModule : InteractionModuleBase<ShardedInteractionContext>
{
    public IConfiguration Configuration { get; set; } = null!;

    [SlashCommand("invite", "Gets an invite URL for the bot to add to your Discord Server.")]
    public async Task InviteUrlAsync()
    {
        var app = await Context.Client.GetApplicationInfoAsync().ConfigureAwait(false);
        var eb = new EmbedBuilder
        {
            Title = "Click here to add me to your guild!",
            Description = "**Note:** You must have the __Manage Server__ permission in the guild you want to add me to.",
            Url = $"https://discord.com/api/oauth2/authorize?permissions=8&client_id={app.Id}&scope=bot%20applications.commands",
            Author = new EmbedAuthorBuilder
            {
                Name = Context.Guild.CurrentUser.Nickname ?? Context.Client.CurrentUser.Username
            },
            ThumbnailUrl = app.IconUrl
        };
        await RespondAsync(embed: eb.Build(), ephemeral: true);
    }

    [SlashCommand("info", "Information about the bot."), RequireBotPermission(ChannelPermission.EmbedLinks)]
    public async Task BotInfoAsync()
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

        await RespondAsync("", embed: eb.Build(), ephemeral: true);
    }

    private static string GetUptime()=> (DateTimeOffset.UtcNow - Process.GetCurrentProcess().StartTime).Humanize(5, true, maxUnit: TimeUnit.Month);
    private static string GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).Megabytes().ToString();
}