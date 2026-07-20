using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using TBRP.DiscordBot.Structs;

namespace TBRP.DiscordBot.Events;

public class AutocompleteInteractionEvent(GatewayClient client,
    ApplicationCommandService<ApplicationCommandContext, AutocompleteInteractionContext> applicationCommandService,
    IServiceProvider serviceProvider)
    : IEventHandler
{
    public void Register()
    {
        client.InteractionCreate += async interaction =>
        {
            if (interaction is not AutocompleteInteraction autocompleteInteraction)
            {
                return;
            }

            DiscordBotClient.Log(
                $"interactionCreate executing autocomplete command={autocompleteInteraction.Data.Name}");

            object? result;
            try
            {
                result = await applicationCommandService.ExecuteAutocompleteAsync(
                    new AutocompleteInteractionContext(autocompleteInteraction, client), serviceProvider);
            }
            catch
            {
                await autocompleteInteraction.SendResponseAsync(InteractionCallback.Autocomplete([]));
                throw;
            }

            if (result is not IFailResult)
            {
                return;
            }

            await autocompleteInteraction.SendResponseAsync(InteractionCallback.Autocomplete([]));
        };
    }
}
