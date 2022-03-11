using Newtonsoft.Json;

namespace WiseOldBot.Features.GeTracker.Api.Models;

public readonly record struct Item (
	[property: JsonProperty("approxProfit")] int? ApproximateProfit,
	[property: JsonProperty("buyLimit")] int BuyLimit,
	[property: JsonProperty("buyPriceCurrent")] bool IsBuyPriceCurrent,
	[property: JsonProperty("buying")] int BuyingPrice,
	[property: JsonProperty("buyingQuantity")] int BuyingQuantity,
	[property: JsonProperty("cachedUntil")] DateTimeOffset CachedUntil,
	[property: JsonProperty("highAlch")] int HighAlchemyValue,
	[property: JsonProperty("icon")] string IconUrl,
	[property: JsonProperty("id")] int? Id,
	[property: JsonProperty("itemId")] int ItemId,
	[property: JsonProperty("lastKnownBuyTime")] DateTimeOffset? LastBuyTime,
	[property: JsonProperty("lastKnownSellTime")] DateTimeOffset? LastSellTime,
	[property: JsonProperty("lowAlch")] int LowAlchemyValue,
	[property: JsonProperty("members")] bool IsMembersItem,
	[property: JsonProperty("name")] string Name,
	[property: JsonProperty("overall")] int AveragePrice,
	[property: JsonProperty("sellPriceCurrent")] bool IsSellPriceCurrent,
	[property: JsonProperty("selling")] int SellingPrice,
	[property: JsonProperty("sellingQuantity")] int SellingQuantity,
	[property: JsonProperty("slug")] string? Slug,
	[property: JsonProperty("tax")] int TaxAmount,
	[property: JsonProperty("updatedAt")] DateTimeOffset? LastUpdatedAt,
	[property: JsonProperty("url")] string Url,
	[property: JsonProperty("wikiUrl")] string? WikiUrl
);