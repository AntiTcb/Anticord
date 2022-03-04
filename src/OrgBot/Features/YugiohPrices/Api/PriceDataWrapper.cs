using Newtonsoft.Json;

namespace OrgBot.Modules.YugiohPrices.Api;

public record PriceDataWrapper(
    [property: JsonProperty("status")] string Status,
    [property: JsonProperty("data")] Data Data);
