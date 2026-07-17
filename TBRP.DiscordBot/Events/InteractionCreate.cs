using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using TBRP.DiscordBot.Structs;

namespace TBRP.DiscordBot.Events;

public class InteractionCreate (GatewayClient client,
    ApplicationCommandService<ApplicationCommandContext> applicationCommandService,
    IServiceProvider serviceProvider)
    : IEventHandler
{
    public void Register()
    {
        client.InteractionCreate += async interaction =>
        {
            Log($"interactionCreate received type={interaction.GetType().Name}");

            if (interaction is not ApplicationCommandInteraction applicationCommandInteraction)
            {
                Log($"interactionCreate ignored type={interaction.GetType().Name}");
                return;
            }

            Log(
                $"interactionCreate executing command name={applicationCommandInteraction.Data.Name} type={applicationCommandInteraction.Data.Type}");

            object? result;
            try
            {
                result = await applicationCommandService.ExecuteAsync(
                    new ApplicationCommandContext(applicationCommandInteraction, client), serviceProvider);
            }
            catch (Exception exception)
            {
                Log($"interactionCreate command threw: {exception}");
                throw;
            }

            Log($"interactionCreate command completed result={result?.GetType().Name ?? "null"}");

            if (result is not IFailResult failResult)
            {
                Log("interactionCreate command succeeded.");
                return;
            }

            Log($"interactionCreate command failed: {failResult.Message}");

            try
            {
                await interaction.SendResponseAsync(InteractionCallback.Message(failResult.Message));
                Log("interactionCreate failure response sent.");
            }
            catch (Exception exception)
            {
                Log($"interactionCreate failure response send failed: {exception.Message}");
            }
        };
    }

    private static void Log(string message)
    {
        Console.WriteLine($"[{DateTimeOffset.UtcNow:O}] {message}");
    }
}
