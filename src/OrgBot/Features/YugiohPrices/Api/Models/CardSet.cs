using Newtonsoft.Json;

namespace OrgBot.Features.YugiohPrices.Api.Models;

public record CardSet(
    [property: JsonProperty("name")] string Name,
    [property: JsonProperty("print_tag")] string PrintTag,
    [property: JsonProperty("rarity")] string Rarity,
    [property: JsonProperty("price_data")] PriceDataWrapper PriceData);

