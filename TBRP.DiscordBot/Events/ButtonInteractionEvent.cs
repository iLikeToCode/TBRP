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
                try
                {
                    await SendErrorResponseAsync(buttonInteraction, exception);
                }
                catch (Exception responseException)
                {
                    DiscordBotClient.Log($"Failed to send button error response: {responseException}");
                }

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
                DiscordBotClient.Log($"Failed to send button failure response: {exception}");
                throw;
            }
        };
    }

    private static async Task SendErrorResponseAsync(ButtonInteraction interaction, Exception exception)
    {
        DiscordBotClient.Log($"Button interaction failed: {exception}");

        const string message = "Something went wrong while handling that button.";

        try
        {
            await interaction.ModifyResponseAsync(m => m.WithContent(message));
            return;
        }
        catch (Exception modifyException)
        {
            DiscordBotClient.Log($"Failed to modify button response after error: {modifyException}");
        }

        try
        {
            await interaction.SendResponseAsync(InteractionCallback.Message(message));
        }
        catch (Exception responseException)
        {
            DiscordBotClient.Log($"Failed to send button error response: {responseException}");
            throw;
        }
    }
}
