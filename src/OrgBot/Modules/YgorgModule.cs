using Discord.Interactions;
using Newtonsoft.Json;

namespace OrgBot.Modules;

public class YGOrgModule : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("news", "Gets the latest article posted to YGOrganization.com")]
    public async Task GetLatestArticleAsync()
    {
        using var c = new HttpClient();
        string json = await c.GetStringAsync("https://ygorganization.com/wp-json/wp/v2/posts/?per_page=1");
        var dummyModel = JsonConvert.DeserializeObject<PostsModel[]>(json)?.FirstOrDefault();
        await RespondAsync(dummyModel?.Link ?? "Latest article could not be found.");
    }

    [SlashCommand("gethelp", "Lists off crisis hotlines")]
    public async Task GetHelpAsync()
    {
        var msg = """
            If you or a loved one needs help, please know you are never alone. Help is available 24/7 at 988 in the USA & Canada.

            For the UK call 0800 689 5652 6PM to midnight every day. 

            Help is available 24/7 in Australia at  Lifeline 13 11 14, Beyond Blue 1300 224 636, or Suicide Call Back Service 1300 659 467

            If your kids need someone to talk to, let them know they can call Kids Help Phone at 1-800-668-6868 or text TALK to 686868. 
            """;
        await RespondAsync(msg);
    }

    internal readonly record struct PostsModel(string Link);
}
