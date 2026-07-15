using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace TBRP.DiscordBot.Structs;

public interface IApplicationCommand
{
    void Register();
}