using Newtonsoft.Json;

namespace OrgBot.Modules.YugiohPrices.Api;

public record Data(
    [property: JsonProperty("listings")] List<string> Listings,
    [property: JsonProperty("prices")] Prices Prices);
