using Newtonsoft.Json;

namespace OrgBot.Features.YGOProDeck.Api.Models;

public record CardSet
{
    [JsonProperty("set_name")]
    public string? Name { get; set; }

    [JsonProperty("set_code")]
    public string? SetCode { get; set; }

    [JsonProperty("set_rarity")]
    public string? SetRarity { get; set; }

    [JsonProperty("set_rarity_code")]
    public string? SetRarityCode { get; set; }

    [JsonProperty("set_price")]
    public string? SetPrice { get; set; }

    [JsonProperty("set_edition")]
    public string? SetEdition { get; set; }

    [JsonProperty("set_price_low")]
    public string? PriceLow { get; set; }
}
