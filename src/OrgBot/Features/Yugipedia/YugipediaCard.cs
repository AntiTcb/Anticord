using Discord;
using Humanizer;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using static Discord.Format;

namespace OrgBot.Features.Yugipedia;

public class YugipediaCard
{
    private string _attribute = null!;
    private static readonly string ZERO_WIDTH_SPACE = "\u200B";

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
    public string Image { get; set; } = null!;
    [JsonProperty("link_arrows")]
    public string LinkArrows { get; set; } = null!;
    [JsonProperty("lore")]
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

    public int? LinkRating => LinkArrows?.Split(',')?.Length;
    public string Description => Format.ResolveMarkup(DescriptionRaw);
    public string ImageUrl => $"https://yugipedia.com/wiki/Special:FilePath/{Image}";
    public string? PendulumEffect => Format.ResolveMarkup(PendulumEffectRaw ?? "");

    public Embed ToEmbed()
    {
        bool isPend = Types?.Contains("Pendulum", StringComparison.OrdinalIgnoreCase) ?? false;
        bool isXyz = Types?.Contains("Xyz", StringComparison.OrdinalIgnoreCase) ?? false;
        bool isLink = Types?.Contains("Link", StringComparison.OrdinalIgnoreCase) ?? false;
        bool isSynchro = Types?.Contains("Synchro", StringComparison.OrdinalIgnoreCase) ?? false;
        bool isSpellOrTrap = !string.IsNullOrWhiteSpace(CardType);

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

        descriptionBuilder.AppendLine($"\n{Bold("Description")}\n{Description}");

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
        };

        return em.Build();
    }

    public static class Format
    {
        const string mediawikiMarkupPattern = @"\[\[(?<NormalText>[^|[\]]+)(?:\|?(?<Vanity>[^|[\]]+)?)\]\]";
        private static readonly Regex markupMatcher = new(mediawikiMarkupPattern, RegexOptions.Compiled);
        private static readonly Regex htmlNewline = new(@"<br\s?/?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string ResolveMarkup(string markup)
        {
            static string ReplaceMatch(Match m) => m.Groups["Vanity"].Success ? m.Groups["Vanity"].Value : m.Groups["NormalText"].Value;

            string resolvedMarkdown = markupMatcher.Replace(markup, ReplaceMatch);
            return htmlNewline.Replace(resolvedMarkdown, "\n");
        }
    }
}
