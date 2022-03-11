using Newtonsoft.Json;

namespace WiseOldBot.Features.GeTracker.Api;

public record ApiResponse<T> (
    [property: JsonProperty("data")] T Data,
    [property: JsonProperty("meta")] Dictionary<string, object> Metadata);
