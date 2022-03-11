using Discord.Addons.Hosting;
using Discord.WebSocket;
using WiseOldBot.Features.GeTracker.Api;
using WiseOldBot.Features.GeTracker.Api.Models;

namespace WiseOldBot.Features.GeTracker;

public class GeTrackerService : DiscordShardedClientService
{
    public ItemMap Items { get; init; }
    public IGeTrackerApi Api { get; init; }

    public GeTrackerService(DiscordShardedClient client, ILogger<DiscordShardedClientService> logger, IGeTrackerApi api) : base(client, logger)
    {
        Items = new ItemMap();
        Api = api;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var items = await Api.GetItemsAsync();

        foreach (var item in TransformItemCollection(items.Data))
        {
            Items.Add(item.Key, item.Value);
        }

        static Dictionary<string, List<GeTrackerItem>> TransformItemCollection(IEnumerable<Item> items)
        {
            return items.GroupBy(static i => i.Name.ToLower())
                .ToDictionary(static g => g.Key, g => g.OrderBy(static i => i.ItemId)
                    .Select(i => new GeTrackerItem(i)).ToList());
        }
    }
}