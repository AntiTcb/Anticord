using Discord;
using Humanizer;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using static Discord.Format;

namespace OrgBot.Features.Yugipedia;

public partial class YugipediaCard
{
    private string _attribute = null!;
    private const string ZERO_WIDTH_SPACE = "\u200B";
    private string image = null!;

    public string Attribute
    {
        get => _attribute;
        set => _attribute = value?.Humanize(LetterCasing.Title) ?? string.Empty;
    }
    [JsonProperty("atk")]
    public string Attack { get; set; } = null!;
    [JsonProperty("card_type")]
    public string CardType { get; set; } = null!;
    [JsonProperty("database_id")]
    public string DatabaseId { get; set; } = null!;
    [JsonProperty("def")]
    public string Defense { get; set; } = null!;
    [JsonProperty("image")]
    public string Image
    {
        get { return image; }
        set
        {
            var match = CardImageRegex().Match(value);
            image = match.Success ? match.Groups["cardname"].Value : value;
        }
    }
    [JsonProperty("link_arrows")]
    public string LinkArrows { get; set; } = null!;
    [JsonProperty("text")]
    public string DescriptionRaw { get; set; } = null!;
    [JsonProperty("level")]
    public int Level { get; set; }
    [JsonProperty("materials")]
    public string Materials { get; set; } = null!;
    [JsonProperty("en_name")]
    public string Name { get; set; } = null!;
    [JsonProperty("password")]
    public string Password { get; set; } = null!;
    [JsonProperty("pendulum_effect")]
    public string PendulumEffectRaw { get; set; } = null!;
    [JsonProperty("pendulum_scale")]
    public int? PendulumScale { get; set; }
    [JsonProperty("property")]
    public string Property { get; set; } = null!;
    [JsonProperty("rank")]
    public int? Rank { get; set; }
    [JsonProperty("tcg_status")]
    public string TcgStatus { get; set; } = null!;
    [JsonProperty("types")]
    public string? Types { get; set; }

    public bool HasDatabaseId => int.TryParse(DatabaseId, out var _);
    public int? LinkRating => LinkArrows?.Split(',')?.Length;
    public string Description => Format.ResolveMarkup(DescriptionRaw ?? "");
    public string ImageUrl => $"https://yugipedia.com/wiki/Special:FilePath/{Image}";
    public string? PendulumEffect => Format.ResolveMarkup(PendulumEffectRaw ?? "");
    public string YGOrgDbLink => $"https://db.ygorganization.com/card#{DatabaseId}";
    public string KonamiDbLink => $"https://www.db.yugioh-card.com/yugiohdb/card_search.action?ope=2&cid={DatabaseId}";
    public string KonamiFAQLink => $"https://www.db.yugioh-card.com/yugiohdb/faq_search.action?ope=4&cid={DatabaseId}&request_locale=ja";
    public string TcgPlayerLink => $"https://tcgplayer.pxf.io/antitcb?u={Uri.EscapeDataString($"https://shop.tcgplayer.com/yugioh/product/show?newSearch=false&IsProductNameExact=false&ProductName={Name}&Type=Cards&orientation=list")}";

    public Embed ToEmbed()
    {
        bool isPend = Types?.Contains("Pendulum", StringComparison.OrdinalIgnoreCase) ?? false;
        bool isXyz = Types?.Contains("Xyz", StringComparison.OrdinalIgnoreCase) ?? false;
        bool isLink = Types?.Contains("Link", StringComparison.OrdinalIgnoreCase) ?? false;
        bool isSynchro = Types?.Contains("Synchro", StringComparison.OrdinalIgnoreCase) ?? false;
        bool isFusion = Types?.Contains("Fusion", StringComparison.OrdinalIgnoreCase) ?? false;
        bool isRitual = Types?.Contains("Ritual", StringComparison.OrdinalIgnoreCase) ?? false;
        bool isEffect = Types?.Contains("Effect", StringComparison.OrdinalIgnoreCase) ?? false;

        bool isSpellOrTrap = !string.IsNullOrWhiteSpace(CardType);

        uint color;
        if (isXyz)
            color = 0x000000;
        else if (isLink)
            color = 0x00008B;
        else if (isSynchro)
            color = 0xEEEEEE;
        else if (isFusion)
            color = 0xA086B7;
        else if (isRitual)
            color = 0x9DB5CC;
        else if (isEffect)
            color = 0xFF8B53;
        else if (isSpellOrTrap)
            color = CardType switch
            {
                "Spell" => 0x1D9E74,
                "Trap" => 0xBC5A84,
                _ => 0x000000
            };
        else
            color = 0xFDE68A;


        var descriptionBuilder = new StringBuilder(ZERO_WIDTH_SPACE);

        if (isSpellOrTrap)
        {
            descriptionBuilder.AppendLine($"{Property} {CardType}");
        }
        else
        {
            if (isXyz)
                descriptionBuilder.Append($"Rank {Rank} | ");
            else if (!isLink)
                descriptionBuilder.Append($"Level {Level} | ");

            descriptionBuilder.AppendLine($"{Attribute?.Trim()} | {Types}");

            descriptionBuilder.Append($"{Bold("ATK")} / {Attack} \t ");

            if (isLink)
                descriptionBuilder.AppendLine($"{Bold("LINK")} / {LinkRating}\n{Bold("Link Arrows:")} {LinkArrows}");
            else
                descriptionBuilder.AppendLine($"{Bold("DEF")} / {Defense}");

            if (isPend)
            {
                descriptionBuilder.AppendLine($"{CustomEmoji.LeftScale} {PendulumScale} / {PendulumScale} {CustomEmoji.RightScale}\n");
                descriptionBuilder.AppendLine($"{Bold("Pendulum Effect:")}\n{PendulumEffect}");
            }
        }

        descriptionBuilder.AppendLine($"\n{Bold("Description")}\n{Description.Replace("<br />", "\n")}");

        var links = new StringBuilder();

        if (int.TryParse(DatabaseId, out var _))
            links.Append($"[Konami DB]({KonamiDbLink}) | [Konami FAQ (Japanese)]({KonamiFAQLink}) | [YGOrg DB]({YGOrgDbLink}) | ");

        links.Append($"[TCGPlayer]({TcgPlayerLink})");

        var em = new EmbedBuilder
        {
            Author = new EmbedAuthorBuilder
            {
                Name = "Yugipedia",
                IconUrl = "https://ms.yugipedia.com//b/bc/Wiki.png",
                Url = "https://yugipedia.com/wiki/Yugipedia"
            },
            Description = descriptionBuilder.ToString(),
            Title = Name,
            ThumbnailUrl = ImageUrl,
            Url = $"https://yugipedia.com/wiki/{Uri.EscapeDataString(Name)}"
        }
        .WithColor(color)
        .AddField("Links", links.ToString());

        return em.Build();
    }

    public static partial class Format
    {
        public static string ResolveMarkup(string markup)
        {
            static string ReplaceMatch(Match m) => m.Groups["Vanity"].Success ? m.Groups["Vanity"].Value : m.Groups["NormalText"].Value;

            string resolvedMarkdown = MediawikiMarkup().Replace(markup, ReplaceMatch);
            return HtmlNewline().Replace(resolvedMarkdown, "\n");
        }

        [GeneratedRegex(@"\[\[(?<NormalText>[^|[\]]+)(?:\|?(?<Vanity>[^|[\]]+)?)\]\]", RegexOptions.Compiled)]
        private static partial Regex MediawikiMarkup();
        [GeneratedRegex(" < br\\s?/?>", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
        private static partial Regex HtmlNewline();
    }

    [GeneratedRegex(@"\d;\s(?<cardname>.+);.+", RegexOptions.Compiled)]
    private static partial Regex CardImageRegex();
}
