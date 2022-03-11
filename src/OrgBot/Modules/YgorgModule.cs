using Discord.Interactions;
using Newtonsoft.Json;

namespace OrgBot.Modules;

public class YgorgModule : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("latest_article", "Gets the latest article posted to YGOrganization.com")]
    public async Task GetLatestArticleAsync()
    {
        using var c = new HttpClient();
        string json = await c.GetStringAsync("https://ygorganization.com/wp-json/wp/v2/posts/?per_page=1");
        var dummyModel = JsonConvert.DeserializeObject<PostsModel[]>(json)?.FirstOrDefault();
        await RespondAsync(dummyModel?.Link ?? "Latest article could not be found.");
    }

    internal readonly record struct PostsModel(string Link);
}
