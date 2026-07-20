using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace TBRP.DiscordBot.Commands.SlashCommands.ErlcCommand;

partial class ErlcCommand
{
    partial class ServerCommand
    {
        [SubSlashCommand("command", "Send a command to the in-game server.")]
        public async Task Command(string command)
        {

            await Context.Interaction.SendResponseAsync(
                InteractionCallback.DeferredMessage()
            );

            var result = await apiClient.ErlcV2.SendCommand(command);

            var embed = new EmbedProperties()
            {
                Title = "In-game Command Result",
                Color = new Color(255, 255, 255),
                Fields =
                [
                    new EmbedFieldProperties()
                    {
                        Name = "Command",
                        Value = command,
                        Inline = true
                    },
                    new EmbedFieldProperties()
                    {
                        Name = "Result",
                        Value = result,
                        Inline = true
                    }
                ]
            };

            await Context.Interaction.ModifyResponseAsync(m => m.AddEmbeds(embed));
        }
    }
}
