using Discord.Interactions;
using Fergun.Interactive.Pagination;
using WiseOldBot.Features.GeTracker;

namespace WiseOldBot.Modules;

public class GeTrackerModule : InteractionModuleBase<ShardedInteractionContext>
{
    public GeTrackerService GeTracker { get; set; } = null!;

    [SlashCommand("price", "Searches GE-Tracker for price information for the specified item.")]
    public async Task GetPriceAsync([Summary("item_name", "Name of the item to search.")] string itemName)
    {
        await DeferAsync();

        var items = GeTracker.Items.Find(itemName).Take(5);

        foreach (var item in items)
        {
            if (item.CachedUntil <= DateTimeOffset.UtcNow || !item.ApproximateProfit.HasValue)
                await item.UpdateAsync(GeTracker.Api);
        }

        if (!items.Any())
        {
            await RespondAsync("Item not found.");
        }
        else if (items.Count() == 1)
        {
            await RespondAsync(embed: items.First().ToDiscordEmbed());
        }
        else
        {
            var paginator = new StaticPaginatorBuilder();
        }
    }
}
