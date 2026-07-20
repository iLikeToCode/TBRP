using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.ComponentInteractions;
using TBRP.DiscordBot.Structs;

namespace TBRP.DiscordBot.Events;

public class InteractionCreate (GatewayClient client)
{
    public void Register()
    {
        client.InteractionCreate += async interaction =>
        {
            Log($"interactionCreate received type={interaction.GetType().Name}");
        };
    }

    private static void Log(string message)
    {
        Console.WriteLine($"[{DateTimeOffset.UtcNow:O}] {message}");
    }
}
