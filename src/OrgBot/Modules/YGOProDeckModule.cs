using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Interactions;
using OrgBot.Features.YGOProDeck.Api;
using OrgBot.Features.YGOProDeck.Api.Models;
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

            foreach (var g in card.CardSets.GroupBy(x => x.SetRarity).OrderBy(x => x.Key))
            {
                var innerSb = new StringBuilder();
                innerSb.AppendLine($"# {g.Key}");

                var ordered = g.OrderBy(x => x.PriceLow).ToList();
                var toShow = new List<CardSet>();

                if (ordered.Count <= 4)
                {
                    toShow = ordered;
                }
                else
                {
                    // Lowest and highest
                    toShow.Add(ordered.First());
                    toShow.Add(ordered.Last());

                    // Up to 2 from the middle
                    var middle = ordered.Skip(1).Take(ordered.Count - 2).ToList();
                    toShow.AddRange(middle.Take(2));
                }

                foreach (var set in toShow)
                {
                    innerSb.AppendLine($"- {set.SetCode,-10} | Low: ${set.PriceLow:C} | Avg: ${set.SetPrice:C}");
                }

                var omitted = ordered.Count - toShow.Count;
                if (omitted > 0)
                {
                    innerSb.AppendLine($"(Omitted {omitted} results...)");
                }

                if (sb.Length < 3900 && innerSb.Length + sb.Length <= 3900)
                    sb.Append(innerSb);
                else
                    break;
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