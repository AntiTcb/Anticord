using Discord.Interactions;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;
using WiseOldBot.Features.GeTracker;
using WiseOldBot.Features.GeTracker.Api;

namespace WiseOldBot.Modules;

public class GeTrackerModule : InteractionModuleBase<ShardedInteractionContext>
{
    public ItemMap ItemMap { get; set; } = null!;
    public IGeTrackerApi Api { get; set; } = null!;
    public InteractiveService FergunInteractiveService { get; set; } = null!;

    [SlashCommand("item", "Searches GE-Tracker for price information for the specified item.")]
    public async Task GetPriceAsync([Summary("item_name", "Name of the item to search.")] string itemName)
    {
        await DeferAsync();

        var items = ItemMap.Find(itemName).Take(10);

        foreach (var item in items)
        {
            if (item.CachedUntil <= DateTimeOffset.UtcNow || !item.ApproximateProfit.HasValue)
                await item.UpdateAsync(Api);
        }

        if (!items.Any())
        {
            await FollowupAsync("Item not found.", ephemeral: true);
        }
        else if (items.Count() == 1)
        {
            await FollowupAsync(embed: items.First().ToDiscordEmbed());
        }
        else
        {
            var pages = items.Select(i => PageBuilder.FromEmbed(i.ToDiscordEmbed()));

            var paginator = new StaticPaginatorBuilder()
                .AddUser(Context.User)
                .WithPages(pages)
                .WithActionOnCancellation(ActionOnStop.DisableInput)
                .Build();

            await FergunInteractiveService.SendPaginatorAsync(paginator,
                interaction: Context.Interaction,
                responseType: Discord.InteractionResponseType.DeferredChannelMessageWithSource,
                timeout: TimeSpan.FromMinutes(5),
                resetTimeoutOnInput: true);
        }
    }
}
