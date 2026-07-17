using NetCord.Gateway;
using NetCord.Services.ApplicationCommands;
using TBRP.DiscordBot.Structs;
using TBRP.ErlcAPI;

namespace TBRP.DiscordBot.Commands.SlashCommands.ERLC;

[SlashCommand("erlc", "ERLC In-game Commands")]
public partial class ErlcCommands(ApiClient apiClient) : ApplicationCommandModule<ApplicationCommandContext>
{
    private static void Log(string message)
    {
        Console.WriteLine($"[{DateTimeOffset.UtcNow:O}] {message}");
    }
}
