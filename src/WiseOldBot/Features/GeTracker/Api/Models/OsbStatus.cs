using Newtonsoft.Json;

namespace WiseOldBot.Features.GeTracker.Api.Models;

public record OsbStatus(
    [property: JsonProperty("status")] string Status,
    [property: JsonProperty("health")] float Health,
    [property: JsonProperty("updateFrequency")] int UpdateFrequency,
    [property: JsonProperty("msg")] string Message
    );
