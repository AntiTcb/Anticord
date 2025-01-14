﻿using Discord;
using Discord.Addons.Hosting;
using Discord.WebSocket;
using Humanizer;
using WiseOldBot.Features.GeTracker.Api;
using WiseOldBot.Features.GeTracker.Api.Models;

namespace WiseOldBot.Features.GeTracker;

public class GeTrackerService : DiscordShardedClientService
{
    private readonly ILogger<DiscordShardedClientService> _logger;

    public IConfiguration Configuration { get; init; }
    public ItemMap Items { get; init; }
    public IGeTrackerApi Api { get; init; }

    public GeTrackerService(DiscordShardedClient client, IConfiguration configuration, ItemMap items, ILogger<DiscordShardedClientService> logger, IGeTrackerApi api)
        :base(client, logger)
    {
        Configuration = configuration;
        Items = items;
        _logger = logger;
        Api = api;
        Api.Token = Configuration["GeTracker:ApiKey"] ?? throw new ArgumentNullException(nameof(Api.Token), "GE Tracker API Key is not configured.");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var items = await Api.GetItemsAsync();

        foreach (var item in TransformItemCollection(items.Data))
        {
            Items.Add(item.Key, item.Value);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            try
            {
                await RebuildItemMapAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error rebuilding item map");
            }
        }
    }

    private async Task RebuildItemMapAsync()
    {
        var downloadedItems = await Api.GetItemsAsync();

        var tempItemMap = TransformItemCollection(downloadedItems.Data);

        var newItems = tempItemMap.Values.SelectMany(i => i)
            .Except(Items.Values.SelectMany(i => i))
            .GroupBy(i => i.Name)
            .ToDictionary(g => g.Key, g => g.OrderBy(x => x.ItemId).ToList());

        if (newItems.Count == 0) return;

        foreach (var item in newItems)
        {
            var wasAdded = Items.TryAdd(item.Key.ToLower(), item.Value);
            if (!wasAdded)
                Items[item.Key.ToLower()] = item.Value;
        }

        string logMessage = $"Item map rebuild! New Items: {string.Join(", ", newItems.Select(i => i.Key))}";

        _logger.LogInformation(logMessage);

        if (Client.GetChannel(Configuration.GetValue<ulong>("Discord:LogChannelId")) is not IMessageChannel logChannel) return;
        await logChannel!.SendMessageAsync(logMessage.Truncate(2000));
    }

    private static Dictionary<string, List<GeTrackerItem>> TransformItemCollection(IEnumerable<Item> items)
    {
        return items.GroupBy(static i => i.Name.ToLower())
            .ToDictionary(static g => g.Key, g => g.OrderBy(static i => i.ItemId)
                .Select(i => new GeTrackerItem(i)).ToList());
    }

}