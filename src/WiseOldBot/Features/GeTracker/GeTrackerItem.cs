using Discord;
using Humanizer.Localisation;
using Humanizer;
using WiseOldBot.Features.GeTracker.Api;
using WiseOldBot.Features.GeTracker.Api.Models;

namespace WiseOldBot.Features.GeTracker;

public readonly record struct GeTrackerItem (
    int? ApproximateProfit,
    int BuyLimit,
    bool IsBuyPriceCurrent,
    int BuyingPrice,
    int BuyingQuantity,
    DateTime CachedUntil,
    int HighAlchemyValue,
    string IconUrl,
    int? Id,
    int ItemId,
    DateTime? LastBuyTime,
    DateTime? LastSellTime,
    int LowAlchemyValue,
    bool IsMembersItem,
    string Name,
    int AveragePrice,
    bool IsSellPriceCurrent,
    int SellingPrice,
    int SellingQuantity,
    int TaxAmount,
    DateTime? LastUpdatedAt,
    string Url,
    string? WikiUrl)
{
    public GeTrackerItem(Item itemModel) : this(
        ApproximateProfit: itemModel.ApproximateProfit,
        BuyLimit: itemModel.BuyLimit,
        IsBuyPriceCurrent: itemModel.IsBuyPriceCurrent,
        BuyingPrice: itemModel.BuyingPrice,
        BuyingQuantity: itemModel.BuyingQuantity,
        CachedUntil: itemModel.CachedUntil,
        HighAlchemyValue: itemModel.HighAlchemyValue,
        IconUrl: itemModel.IconUrl,
        Id: itemModel.Id,
        ItemId: itemModel.ItemId,
        LastBuyTime: itemModel.LastBuyTime,
        LastSellTime: itemModel.LastSellTime,
        LowAlchemyValue: itemModel.LowAlchemyValue,
        IsMembersItem: itemModel.IsMembersItem,
        Name: itemModel.Name,
        AveragePrice: itemModel.AveragePrice,
        IsSellPriceCurrent: itemModel.IsSellPriceCurrent,
        SellingPrice: itemModel.SellingPrice,
        SellingQuantity: itemModel.SellingQuantity,
        TaxAmount: itemModel.TaxAmount,
        LastUpdatedAt: itemModel.LastUpdatedAt,
        Url: itemModel.Url,
        WikiUrl: itemModel.WikiUrl)
    { }

    public string OsrsExchangeUrl => $"http://services.runescape.com/m=itemdb_oldschool/Runescape/viewitem?obj={ItemId}";
    public string RsbExchangeUrl => $"https://rsbuddy.com/exchange?id={ItemId}";
    public string OsrsWikiUrl => $"https://os.rs.wiki/w/{Name.Replace(" ", "_")}";
    public string SpriteUrl => $"https://services.runescape.com/m=itemdb_oldschool/obj_sprite.gif?id={ItemId}";
    public string OsrsWikiExchangeUrl => $"https://prices.runescape.wiki/osrs/item/{ItemId}";

    public GeTrackerItem Update(Item updatedItem)
    {
        if (!updatedItem.Equals(this)) return this;

        return this with
        {
            AveragePrice = updatedItem.AveragePrice,
            BuyLimit = updatedItem.BuyLimit,
            BuyingPrice = updatedItem.BuyingPrice,
            BuyingQuantity = updatedItem.BuyingQuantity,
            CachedUntil = updatedItem.CachedUntil,
            SellingPrice = updatedItem.SellingPrice,
            SellingQuantity = updatedItem.SellingQuantity,
            ApproximateProfit = updatedItem.ApproximateProfit,
            LastUpdatedAt = updatedItem.LastUpdatedAt
        };
    }

    public async Task<GeTrackerItem> UpdateAsync(IGeTrackerApi api)
    {
        var response = await api.GetItemAsync(ItemId);

        return Update(response.Data);
    }

    public Embed ToDiscordEmbed()
    {
        var eb = new EmbedBuilder()
        .WithTitle($"{Name} (ID: {ItemId})")
        .WithDescription($"[GE-Tracker]({Url}) | [OSRS Exchange]({OsrsExchangeUrl}) | [RSB Exchange]({RsbExchangeUrl}) | [2007 Wiki]({WikiUrl ?? OsrsWikiUrl}) | [Wiki Exchange]({OsrsWikiExchangeUrl})")
        .WithUrl(Url)
        .WithThumbnailUrl(SpriteUrl)
        .WithAuthor("GE-Tracker", "https://cdn.discordapp.com/avatars/372857710229848064/f997ce46943f41a18bb089f6b41954af.png?size=128", Url)
        .AddField("Buying", $"Price: {$"{BuyingPrice:N0}"} {CustomEmoji.Gold}\nQuantity: {$"{BuyingQuantity:N0}"}", true)
        .AddField("Average", $"Price: {$"{AveragePrice:N0}"} {CustomEmoji.Gold}", true)
        .AddField("Selling", $"Price: {$"{SellingPrice:N0}"} {CustomEmoji.Gold}\nQuantity: {$"{SellingQuantity:N0}"}", true);

        if (ApproximateProfit.HasValue)
            eb.AddField("Approx. Profit", $"{$"{ApproximateProfit:N0}"} {CustomEmoji.Gold}", true);

        eb.AddField("Alch", $"Low: {$"{LowAlchemyValue:N0}"} {CustomEmoji.Gold}\nHigh: {$"{HighAlchemyValue:N0}"} {CustomEmoji.Gold}", true)
          .AddField("Buy Limit", BuyLimit.ToString("N0"), true);

        return eb.Build();
    }

    public bool Equals(GeTrackerItem obj) => obj.ItemId.Equals(ItemId);
    public override int GetHashCode() => ItemId.GetHashCode();
}