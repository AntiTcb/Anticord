using Newtonsoft.Json;

namespace OrgBot.Features.YugiohPrices.Api.Models;

public record Data(
    [property: JsonProperty("listings")] List<string> Listings,
    [property: JsonProperty("prices")] Prices Prices);
