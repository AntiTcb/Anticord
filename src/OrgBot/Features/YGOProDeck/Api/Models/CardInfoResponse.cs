using Newtonsoft.Json;

namespace OrgBot.Features.YGOProDeck.Api.Models;

public record CardInfoResponse
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("typeline")]
    public List<string> TypeLine { get; set; } = [];

    [JsonProperty("type")]
    public string Type { get; set; } = null!;

    [JsonProperty("humanReadableCardType")]
    public string HumanReadableCardType { get; set; } = null!;

    [JsonProperty("frameType")]
    public string FrameType { get; set; } = null!;

    [JsonProperty("desc")]
    public string? Description { get; set; }

    [JsonProperty("pend_desc")]
    public string? PendulumDescription { get; set; }

    [JsonProperty("monster_desc")]
    public string? MonsterDescription { get; set; }

    [JsonProperty("race")]
    public string Race { get; set; } = null!;

    [JsonProperty("atk")]
    public int? Attack { get; set; }

    [JsonProperty("def")]
    public int? Defense { get; set; }

    [JsonProperty("level")]
    public int? Level { get; set; }

    [JsonProperty("scale")]
    public int? Scale { get; set; }

    [JsonProperty("linkval")]
    public int? LinkValue { get; set; }

    [JsonProperty("linkmarkers")]
    public List<string> LinkMarkers { get; set; } = [];

    [JsonProperty("attribute")]
    public string? Attribute { get; set; }

    [JsonProperty("archetype")]
    public string? Archetype { get; set; }

    [JsonProperty("card_sets")]
    public List<CardSet> CardSets { get; set; } = [];

    [JsonProperty("card_images")]
    public List<CardImage> CardImages { get; set; } = [];

    public string TcgPlayerLink => $"https://partner.tcgplayer.com/antitcb?u={Uri.EscapeDataString($"https://shop.tcgplayer.com/yugioh/product/show?newSearch=false&IsProductNameExact=false&ProductName={Name}&Type=Cards&orientation=list")}";
}
