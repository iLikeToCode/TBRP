using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.ComponentInteractions;
using TBRP.DiscordBot.Structs;

namespace TBRP.DiscordBot.Events;

public class ButtonInteractionEvent (GatewayClient client,
    ComponentInteractionService<ButtonInteractionContext> buttonService,
    IServiceProvider serviceProvider)
    : IEventHandler
{
    public void Register()
    {
        client.InteractionCreate += async interaction =>
        {
            if (interaction is not ButtonInteraction buttonInteraction)
            {
                return;
            }

            DiscordBotClient.Log(
                $"interactionCreate executing button id={buttonInteraction.Data.CustomId}");

            object? result;
            try
            {
                result = await buttonService.ExecuteAsync(
                    new ButtonInteractionContext(buttonInteraction, client), serviceProvider);
            }
            catch (Exception exception)
            {
                throw;
            }

            if (result is not IFailResult failResult)
            {
                return;
            }

            try
            {
                await interaction.SendResponseAsync(InteractionCallback.Message(failResult.Message));
            }
            catch (Exception exception)
            {
            }
        };
    }
}
