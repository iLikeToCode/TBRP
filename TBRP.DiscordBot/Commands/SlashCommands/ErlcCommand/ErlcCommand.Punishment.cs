using NetCord.Services.ApplicationCommands;
using TBRP.Api;

namespace TBRP.DiscordBot.Commands.SlashCommands.ErlcCommand;

partial class ErlcCommand
{
    [SubSlashCommand("punishment", "Punishment commands")]
    public partial class PunishmentCommand(ApiClient apiClient) : ApplicationCommandModule<ApplicationCommandContext>
    {
        
    }
}
