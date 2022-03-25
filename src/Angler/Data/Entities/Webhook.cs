using Discord.Webhook;
using System.Text.RegularExpressions;

namespace Angler.Data.Entities;

public class Webhook
{
    private static readonly Regex _webhookRegex = new(@".*webhooks\/(\d*)\/(.*)");
    private readonly Lazy<DiscordWebhookClient> _client;

    public Website Site { get; set; }
    public ulong Id { get; set; }
    public string Token { get; set; } = null!;

    public Webhook()
    {
        _client = new Lazy<DiscordWebhookClient>(() => new(Id, Token));
    }

    public Webhook(ulong id, string token, Website site) : this()
    {
        Id = id;
        Token = token;
        Site = site;
    }
    public Webhook(string webhookUrl, Website site) : this()
    {
        (Id, Token) = ParseUrl(webhookUrl);
        Site = site;
    }
    public static (ulong Id, string Token) ParseUrl(string url)
    {
        var groups = _webhookRegex.Match(url).Groups;
        ulong id = ulong.Parse(groups[1].Value);
        string token = groups[2].Value;
        return (id, token);
    }

    public DiscordWebhookClient GetClient() => _client.Value;
}
