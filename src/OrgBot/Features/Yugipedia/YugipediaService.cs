﻿using System.Text.RegularExpressions;
using Newtonsoft.Json;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;

namespace OrgBot.Features.Yugipedia;

public class YugipediaService
{
    public WikiSite Site { get; set; }

    private static readonly Regex _cardTableParser = new(@"\|\s([\w|_]+)\s*=\s+(.*)", RegexOptions.Compiled);
    private static readonly Regex _searchFiltering = new(@"\((anime|BAM)\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public YugipediaService(WikiSite site)
        => Site = site;

    public async Task<YugipediaCard?> GetCardAsync(string cardName)
    {
        var requestProcess = "Start.";
        try
        {
            var searchResults = (await Site.OpenSearchAsync(cardName)).Where(r => !_searchFiltering.IsMatch(r.Url));
            requestProcess = "After search.";

            if (!searchResults.Any()) return null;

            cardName = searchResults.FirstOrDefault().Title;

            if (cardName is null) return null;

            var page = new WikiPage(Site, cardName);
            await page.RefreshAsync(PageQueryOptions.FetchContent | PageQueryOptions.ResolveRedirects);
            requestProcess = "Fetch Content & Resolve Redirects.";

            if (string.IsNullOrEmpty(page.Content) || page.NamespaceId != 0 || !page.Content.Contains("{{CardTable2")) return null;

            var propDict = new Dictionary<string, string>
            {
                { "en_name", page.Title }
            };

            var props = _cardTableParser.Matches(page.Content)
                .OfType<Match>()
                .Select(m => (m.Groups[1].Value, m.Groups[2].Value));

            foreach (var (key, value) in props)
                propDict[key] = key == "image" && value.Contains("1;") ? Regex.Replace(value, @"(?:1;\s+)([^;\n]+)(?:.*)", "$1") : value;

            // TODO: This is so hackily bad.
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var card = JsonConvert.DeserializeObject<YugipediaCard>(JsonConvert.SerializeObject(propDict, settings), settings);

            return card;
        }
        catch (TimeoutException e)
        {
            var ex = new TimeoutException("The Yugipedia API timed out; please try again later.", e);
            ex.Data.Add("requestProcess", requestProcess);
            throw ex;
        }
    }
}
