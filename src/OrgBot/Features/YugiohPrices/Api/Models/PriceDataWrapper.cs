using Newtonsoft.Json;

namespace OrgBot.Features.YugiohPrices.Api.Models;

public record PriceDataWrapper(
    [property: JsonProperty("status")] string Status,
    [property: JsonProperty("data")] Data? Data);
