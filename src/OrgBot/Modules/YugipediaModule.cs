using Discord;
using Discord.Interactions;
using OrgBot.Features.Yugipedia;

namespace OrgBot.Modules;

public class YugipediaModule : InteractionModuleBase<ShardedInteractionContext>
{
    public YugipediaService Yugipedia { get; set; } = null!;
    public IConfiguration Configuration { get; set; } = null!;

    [SlashCommand("card", "Search Yugipedia for the specified card and shows its information.")]
    [RequireBotPermission(ChannelPermission.EmbedLinks)]
    public async Task GetCardAsync([Summary("card_name", "Card name, case insensitive")] string cardName)
    {
        try
        {
            var card = await Yugipedia.GetCardAsync(cardName);
            if (card is null)
            {
                await RespondAsync("Couldn't find a matching card in the TCG/OCG.");
                return;
            }

            await RespondAsync(embed: card.ToEmbed());
        }
        catch (TimeoutException e) when (e.Message == "The Yugipedia API timed out; please try again.")
        {
            await RespondAsync(e.Message);

            var eb = new EmbedBuilder
            {
                Title = "Yugipedia timeout",
                Description = e.Data["requestProcess"]!.ToString()
            }
            .AddField("Caller", Context.User.ToString(), true)
            .AddField("Guild", Context.Channel is IGuildChannel ? Context.Guild.ToString() : "DM")
            .AddField("Channel", Context.Channel.Id);

            await (Context.Client.GetChannel(Configuration.GetValue<ulong>("Discord:TestGuildId")) as IMessageChannel)!.SendMessageAsync(embed: eb.Build());
        }
    }
}
