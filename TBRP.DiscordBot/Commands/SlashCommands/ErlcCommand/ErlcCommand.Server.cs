using NetCord.Services.ApplicationCommands;
using TBRP.Api;

namespace TBRP.DiscordBot.Commands.SlashCommands.ErlcCommand;

partial class ErlcCommand
{
    [SubSlashCommand("server", "Server commands")]
    public partial class ServerCommand(ApiClient apiClient) : ApplicationCommandModule<ApplicationCommandContext>
    {
        
    }
}
