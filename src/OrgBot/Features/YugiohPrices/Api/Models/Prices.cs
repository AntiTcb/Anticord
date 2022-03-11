using Newtonsoft.Json;

namespace OrgBot.Features.YugiohPrices.Api.Models;

public record Prices(
    [property: JsonProperty("high", NullValueHandling = NullValueHandling.Ignore)] double? High,
    [property: JsonProperty("low", NullValueHandling = NullValueHandling.Ignore)] double? Low,
    [property: JsonProperty("average", NullValueHandling = NullValueHandling.Ignore)] double? Average,
    [property: JsonProperty("shift", NullValueHandling = NullValueHandling.Ignore)] double? Shift,
    [property: JsonProperty("shift_3", NullValueHandling = NullValueHandling.Ignore)] double? Shift3,
    [property: JsonProperty("shift_7", NullValueHandling = NullValueHandling.Ignore)] double? Shift7,
    [property: JsonProperty("shift_21", NullValueHandling = NullValueHandling.Ignore)] double? Shift21,
    [property: JsonProperty("shift_30", NullValueHandling = NullValueHandling.Ignore)] double? Shift30,
    [property: JsonProperty("shift_90", NullValueHandling = NullValueHandling.Ignore)] double? Shift90,
    [property: JsonProperty("shift_180", NullValueHandling = NullValueHandling.Ignore)] double? Shift180,
    [property: JsonProperty("shift_365", NullValueHandling = NullValueHandling.Ignore)] double? Shift365,
    [property: JsonProperty("updated_at", NullValueHandling = NullValueHandling.Ignore)] DateTimeOffset UpdatedAt
);