using Discord;
using Discord.Interactions;
using OrgBot.Features.YugiohPrices.Api;

namespace OrgBot.Modules;

public class YugiohPricesModule : InteractionModuleBase<ShardedInteractionContext>
{
    public IYugiohPricesApi Api { get; set; } = null!;

    [SlashCommand("cardprice", "Looks up the top 5 prices of a Yu-Gi-Oh! card. Card name must be exact.")]
    [RequireBotPermission(ChannelPermission.EmbedLinks)]
    public async Task GetCardPriceAsync([Summary("card_name", "Card name. Must be exact.")] string cardName, [Summary("number_of_sets", "Maximum number of sets to display.")]int numberOfSets = 5)
    {
        var cardPrices = await Api.GetCardPriceAsync(cardName);

        if (cardPrices.Status == "fail" || cardPrices.Data is null)
        {
            await RespondAsync("Could not find card information. Double check your input, as the card name MUST be an exact match.");
            return;
        }

        var eb = new EmbedBuilder()
            .WithTitle(cardPrices.Data.First().Name)
            .WithAuthor("YugiohPrices.com", "https://yugiohprices.com/img/banner.png", "https://yugiohprices.com")
            .WithThumbnailUrl("https://yugiohprices.com/img/banner.png")
            .WithCurrentTimestamp()
            .WithFooter("Prices brought to you courtesy of YugiohPrices.com");

        var setData = cardPrices.Data
            .OrderByDescending(p => p.PriceData.Data.Prices.Average)
            .Take(numberOfSets)
            .ToDictionary(p => $"{p.Name} | {p.PrintTag} | {p.Rarity}", p => p.PriceData.Data.Prices);

        foreach (var (SetName, SetPrices) in setData)
        {
            var (High, Low, Average) = (SetPrices.High, SetPrices.Low, SetPrices.Average);
            eb.AddField(SetName, $"Low: {Low:C} | Avg: {Average:C} | High: {High:C}");
        }

        await RespondAsync(embed: eb.Build());
    }
}