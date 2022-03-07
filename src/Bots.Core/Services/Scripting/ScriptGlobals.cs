using Discord.Interactions;
using Discord.WebSocket;

namespace Bots.Core.Services.Scripting;

public class ScriptGlobals
{
    public ShardedInteractionContext Context { get; set; }
    public InteractionService Commands { get; set; }

    public DiscordShardedClient Client => Context.Client;
    public ISocketMessageChannel Channel => Context.Channel;
    public SocketGuild Guild => Context.Guild;
    public SocketTextChannel? GuildChannel => Context.Channel as SocketTextChannel;
    public DiscordSocketClient Shard => Client.GetShardFor(Guild);
    public SocketUser User => Context.User;
    public SocketGuildUser? GuildUser => Context.User as SocketGuildUser;

    public ScriptGlobals(ShardedInteractionContext ctx, InteractionService cmds)
    {
        Context = ctx;
        Commands = cmds;
    }
}
