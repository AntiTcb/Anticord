using Newtonsoft.Json;

namespace OrgBot.Features.YugiohPrices.Api;

public record ApiResponse<T>(
    [property: JsonProperty("status")] string Status,
    [property: JsonProperty("data")] T[]? Data,
    [property: JsonProperty("message")] string? Message);
