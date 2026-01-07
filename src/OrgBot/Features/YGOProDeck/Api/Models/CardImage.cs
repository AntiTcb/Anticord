using Newtonsoft.Json;

namespace OrgBot.Features.YGOProDeck.Api.Models;

public record CardImage
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("image_url")]
    public string ImageUrl { get; set; } = null!;

    [JsonProperty("image_url_small")]
    public string ImageUrlSmall { get; set; } = null!;

    [JsonProperty("image_url_cropped")]
    public string ImageUrlCropped { get; set; } = null!;
}
