using Discord;
using Discord.Interactions;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;

namespace Bots.Core.Services.Scripting;

public class ScriptService
{
    private readonly InteractionService _commands;
    private readonly ILogger<ScriptService> _logger;
    private readonly IEnumerable<Assembly> _assemblies;
    private readonly IEnumerable<string> _namespaces;
    private readonly ScriptOptions _options;

    public ScriptService(InteractionService commands, ILogger<ScriptService> logger)
    {
        _commands = commands;
        _logger = logger;
        _assemblies = GetAssemblies();
        _namespaces = GetNamespaces();
        _options = ScriptOptions.Default.AddReferences(_assemblies).AddImports(_namespaces);
    }

    public async Task EvaluteAsync(ShardedInteractionContext ctx, string expr)
    {
        var sw = Stopwatch.StartNew();
        expr = expr.Trim('`');
        var globals = new ScriptGlobals(ctx, _commands);
        await ctx.Interaction.RespondAsync("**Evaluating...**", ephemeral: true);
        var working = await ctx.Interaction.GetOriginalResponseAsync();
        try
        {
            object eval = await CSharpScript.EvaluateAsync(expr, _options, globals, typeof(ScriptGlobals));
            sw.Stop();
            var eb = new EmbedBuilder
            {
                Title = "Result:",
                Description = eval.ToString(),
                Color = Color.Green,
                Footer = new EmbedFooterBuilder { Text = $"Elapsed: {sw.Elapsed.Humanize()}" }
            };

            await working.ModifyAsync(x => { x.Content = "Done!"; x.Embed = eb.Build(); });
        }
        catch (Exception e)
        {
            await working.ModifyAsync(x => x.Content = Format.Code(e.ToString()));
        }
    }

    protected virtual IEnumerable<string> GetNamespaces()
    {
        var assemblies = new[]
        {
            Assembly.GetEntryAssembly(),
            typeof(DiscordBotBase).GetTypeInfo().Assembly,
            typeof(Discord.Webhook.DiscordWebhookClient).GetTypeInfo().Assembly,
            typeof(To).GetTypeInfo().Assembly
        };
        var manualNamespaces = new[]
        {
            "System", "System.Linq", "System.Threading.Tasks", "System.Reflection", "System.Collections", "System.Collections.Generic",
            "System.IO", "System.Math", "System.Diagnostics",

            "Discord", "Discord.Commands", "Discord.Rest", "Discord.WebSocket", "Discord.Webhook",
        };
        var customNamespaces = new HashSet<string>(assemblies.SelectMany(a => a!.GetTypes().Select(t => t.Namespace!)).Where(c => !string.IsNullOrEmpty(c)));
        return manualNamespaces.Concat(customNamespaces);
    }

    protected virtual IEnumerable<Assembly> GetAssemblies()
    {
        var assemblies = Assembly.GetEntryAssembly()!.GetReferencedAssemblies();
        foreach (var a in assemblies)
        {
            var asm = Assembly.Load(a);
            yield return asm;
        }
        yield return Assembly.GetEntryAssembly()!;
        yield return typeof(ILookup<string, string>).GetTypeInfo().Assembly;
    }
}
