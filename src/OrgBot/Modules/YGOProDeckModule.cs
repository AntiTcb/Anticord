using System.Text;
using Discord;
using Discord.Interactions;
using OrgBot.Features.YGOProDeck.Api;
using RestEase;

namespace OrgBot.Modules;

public class YGOProDeckModule : InteractionModuleBase<ShardedInteractionContext>
{
    public IYGOProDeckApi Api { get; set; } = null!;

    [SlashCommand("cardprice", "Looks up the prices of a Yu-Gi-Oh! card from TCGPlayer. Card name must be exact.")]
    [RequireBotPermission(ChannelPermission.EmbedLinks)]
    public async Task GetCardPriceAsync([Summary("card_name", "Card name. Must be exact.")] string cardName)
    {
        try
        {
            var searchResult = await Api.GetCardPriceAsync(cardName);
            var card = searchResult.Data?.FirstOrDefault();

            if (card is null || card.CardSets.Count == 0)
            {
                await RespondAsync("Could not find card information or price data. Double check the name is correct and not abbreviated and try again.", ephemeral: true);
                return;
            }

            var eb = new EmbedBuilder()
                .WithTitle(card.Name)
                .WithAuthor("TCGPlayer.com", "https://cdn.brandfetch.io/id5-LF3HOp/w/820/h/345/theme/dark/logo.png?c=1dxbfHSJFAPEGdCLU4o5B", $"https://partner.tcgplayer.com/antitcb?u={Uri.EscapeDataString("https://tcgplayer.com")}")
                .WithThumbnailUrl(card.CardImages.FirstOrDefault()?.ImageUrl ?? card.CardImages.FirstOrDefault()?.ImageUrlSmall ?? card.CardImages.FirstOrDefault()?.ImageUrlCropped ?? "")
                .WithCurrentTimestamp()
                .WithUrl(card.TcgPlayerLink)
                .WithFooter("Prices brought to you courtesy of TCGPlayer.com");

            var sb = new StringBuilder();

            foreach (var set in card.CardSets.Take(25))
            {
                if (set is null) continue;

                sb.AppendLine($"# {set.SetCode} | {set.SetRarity}\n - Low: ${set.PriceLow:C} | Avg: ${set.SetPrice:C}");
            }

            eb.WithDescription(Format.Code(sb.ToString(), "md") + "\n" + Format.Url("Buy the card now at TCGPlayer!", card.TcgPlayerLink));

            await RespondAsync(embed: eb.Build());
        }
        catch (ApiException e) when (e.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            await RespondAsync("Could not find card information or price data. Double check the name is correct and not abbreviated and try again.", ephemeral: true);
        }
    }
}