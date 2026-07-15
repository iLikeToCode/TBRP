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
            if (interaction is not ApplicationCommandInteraction applicationCommandInteraction)
                return;

            var result = await applicationCommandService.ExecuteAsync(
                new ApplicationCommandContext(applicationCommandInteraction, client), serviceProvider);

            if (result is not IFailResult failResult)
                return;

            try
            {
                await interaction.SendResponseAsync(InteractionCallback.Message(failResult.Message));
            }
            catch
            {
            }
        };
    }
}