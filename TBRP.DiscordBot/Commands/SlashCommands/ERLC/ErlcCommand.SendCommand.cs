using System.Text;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace TBRP.DiscordBot.Commands.SlashCommands.ERLC;

partial class ErlcCommands
{
    [SubSlashCommand("command", "Send a command to the in-game server.")]
    public async Task Command(string command)
    {
        Log($"ERLC command requested command=\"{command}\"");
        Log("ERLC command defer starting.");

        await Context.Interaction.SendResponseAsync(
            InteractionCallback.DeferredMessage()
        );

        Log("ERLC command defer completed.");
        Log("ERLC command API call starting.");

        var result = await apiClient.ErlcV2.SendCommand(command);

        Log($"ERLC command API call completed result=\"{result}\"");
        
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

        Log("ERLC command response modify starting.");
        await Context.Interaction.ModifyResponseAsync(m => m.AddEmbeds(embed));
        Log("ERLC command response modify completed.");
    }
}
