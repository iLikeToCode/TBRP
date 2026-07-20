using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using TBRP.DiscordBot.Structs;

namespace TBRP.DiscordBot.Events;

public class SlashCommandEvent (GatewayClient client,
    ApplicationCommandService<ApplicationCommandContext, AutocompleteInteractionContext> applicationCommandService,
    IServiceProvider serviceProvider)
    : IEventHandler
{
    public void Register()
    {
        client.InteractionCreate += async interaction =>
        {
            if (interaction is not ApplicationCommandInteraction applicationCommandInteraction)
            {
                return;
            }

            DiscordBotClient.Log(
                $"interactionCreate executing command name={applicationCommandInteraction.Data.Name} type={applicationCommandInteraction.Data.Type}");

            object? result;
            try
            {
                result = await applicationCommandService.ExecuteAsync(
                    new ApplicationCommandContext(applicationCommandInteraction, client), serviceProvider);
            }
            catch (Exception exception)
            {
                try
                {
                    await SendErrorResponseAsync(applicationCommandInteraction, exception);
                }
                catch (Exception responseException)
                {
                    DiscordBotClient.Log($"Failed to send command error response: {responseException}");
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
                DiscordBotClient.Log($"Failed to send command failure response: {exception}");
                throw;
            }
        };
    }

    private static async Task SendErrorResponseAsync(ApplicationCommandInteraction interaction, Exception exception)
    {
        DiscordBotClient.Log($"Slash command failed: {exception}");

        const string message = "Something went wrong while running that command.";

        try
        {
            await interaction.ModifyResponseAsync(m => m.WithContent(message));
            return;
        }
        catch (Exception modifyException)
        {
            DiscordBotClient.Log($"Failed to modify command response after error: {modifyException}");
        }

        try
        {
            await interaction.SendResponseAsync(InteractionCallback.Message(message));
        }
        catch (Exception responseException)
        {
            DiscordBotClient.Log($"Failed to send command error response: {responseException}");
            throw;
        }
    }
}
