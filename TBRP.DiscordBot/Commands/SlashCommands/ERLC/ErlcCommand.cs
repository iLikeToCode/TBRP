using NetCord.Gateway;
using NetCord.Services.ApplicationCommands;
using TBRP.DiscordBot.Structs;
using TBRP.ErlcAPI;

namespace TBRP.DiscordBot.Commands.SlashCommands.ERLC;

[SlashCommand("erlc", "ERLC In-game Commands")]
public partial class ErlcCommands(GatewayClient client, ApiClient apiClient, ApplicationCommandService<ApplicationCommandContext> applicationCommandService) : ApplicationCommandModule<ApplicationCommandContext>
{
    
}