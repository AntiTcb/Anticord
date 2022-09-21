using System.Reflection;
using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bots.Core.Services;

internal class InteractionHandler : DiscordShardedClientService
{
    private readonly IServiceProvider _provider;
    private readonly InteractionService _interactionService;
    private readonly IHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public InteractionHandler(DiscordShardedClient client, ILogger<DiscordShardedClientService> logger, IServiceProvider provider, InteractionService interactionService, IHostEnvironment environment, IConfiguration configuration) : base(client, logger)
    {
        _provider = provider;
        _interactionService = interactionService;
        _environment = environment;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Process the InteractionCreated payloads to execute Interactions commands
        Client.InteractionCreated += HandleInteraction;

        // Process the command execution results 
        _interactionService.SlashCommandExecuted += SlashCommandExecuted;
        _interactionService.ContextCommandExecuted += ContextCommandExecuted;
        _interactionService.ComponentCommandExecuted += ComponentCommandExecuted;

        await AddModulesFromAssemblyAsync(Assembly.GetEntryAssembly()!);
        await AddModulesFromAssemblyAsync(typeof(DiscordBotBase).GetTypeInfo().Assembly);
        await Client.WaitForReadyAsync(stoppingToken);

        // If DOTNET_ENVIRONMENT is set to development, only register the commands to a single guild
        if (_environment.IsDevelopment())
            await _interactionService.RegisterCommandsToGuildAsync(_configuration.GetValue<ulong>("Discord:TestGuildId"));
        else
            await _interactionService.RegisterCommandsGloballyAsync();
    }

    private async Task AddModulesFromAssemblyAsync(Assembly assembly)
    {
        var modules = await _interactionService.AddModulesAsync(assembly, _provider);
        foreach (var module in modules)
        {
            Logger.LogInformation($"Loaded module {module.Name}, with {module.ComponentCommands.Count} Component Commands, {module.SlashCommands.Count} Slash Commands, "
                + $"{module.ModalCommands.Count} Modal Commands, {module.AutocompleteCommands.Count} Autocomplete Commands, and {module.ContextCommands.Count} Context Commands.");
        }
    }

    private Task ComponentCommandExecuted(ComponentCommandInfo commandInfo, IInteractionContext context, IResult result)
    {
        if (!result.IsSuccess)
        {
            Logger.LogError($"[{nameof(ComponentCommandExecuted)}] {result.Error} {result.ErrorReason}");
            switch (result.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    // implement
                    break;
                case InteractionCommandError.UnknownCommand:
                    // implement
                    break;
                case InteractionCommandError.BadArgs:
                    // implement
                    break;
                case InteractionCommandError.Exception:
                    // implement
                    break;
                case InteractionCommandError.Unsuccessful:
                    // implement
                    break;
                default:
                    break;
            }
        }

        return Task.CompletedTask;
    }

    private Task ContextCommandExecuted(ContextCommandInfo context, IInteractionContext arg2, IResult result)
    {
        if (!result.IsSuccess)
        {
            Logger.LogError($"[{nameof(ContextCommandExecuted)}] {result.Error} {result.ErrorReason}");
            switch (result.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    // implement
                    break;
                case InteractionCommandError.UnknownCommand:
                    // implement
                    break;
                case InteractionCommandError.BadArgs:
                    // implement
                    break;
                case InteractionCommandError.Exception:
                    // implement
                    break;
                case InteractionCommandError.Unsuccessful:
                    // implement
                    break;
                default:
                    break;
            }
        }

        return Task.CompletedTask;
    }

    private async Task SlashCommandExecuted(SlashCommandInfo commandInfo, IInteractionContext context, IResult result)
    {
        if (!result.IsSuccess)
        {
            Logger.LogError($"[{nameof(SlashCommandExecuted)}] {result.Error} {result.ErrorReason}");
            switch (result.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    // implement
                    break;
                case InteractionCommandError.UnknownCommand:
                    // implement
                    break;
                case InteractionCommandError.BadArgs:
                    // implement
                    break;
                case InteractionCommandError.Exception:
                    await context.Interaction.RespondAsync("Oops, I ran into an exception. Please try the command again.");
                    var logChannel = await context.Client.GetChannelAsync(_configuration.GetValue<ulong>("Discord:LogChannelId")) as IMessageChannel;
                    await logChannel!.SendMessageAsync($"{commandInfo.Name} command failed", embed:
                        new EmbedBuilder()
                        .WithTitle(result.Error.ToString())
                        .WithDescription($"{result.ErrorReason} {((result is ExecuteResult exResult1) ? exResult1.Exception.StackTrace?.ToString() : "None")}")
                        .AddField("Guild", context.Guild.Name)
                        .AddField("User", context.User.ToString())
                        .AddField("Exception", (result is ExecuteResult exResult2) ? exResult2.Exception.ToString() : "None")
                        .Build());
                    break;
                case InteractionCommandError.Unsuccessful:
                    // implement
                    break;
                default:
                    break;
            }
        }
    }


    private async Task HandleInteraction(SocketInteraction arg)
    {
        try
        {
            var ctx = new ShardedInteractionContext(Client, arg);
            await _interactionService.ExecuteCommandAsync(ctx, _provider);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception occurred whilst attempting to handle interaction.");

            if (arg.Type == InteractionType.ApplicationCommand)
            {
                var msg = await arg.GetOriginalResponseAsync();
                await msg.DeleteAsync();
            }
        }
    }
}
