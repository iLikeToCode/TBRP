using NetCord.Services.ApplicationCommands;
using TBRP.Api;

namespace TBRP.DiscordBot.Commands.SlashCommands.ErlcCommand;

[SlashCommand("erlc", "ERLC In-game Commands")]
public partial class ErlcCommand(ApiClient apiClient) : ApplicationCommandModule<ApplicationCommandContext>
{
    
}
