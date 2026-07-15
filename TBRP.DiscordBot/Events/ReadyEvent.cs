using NetCord;
using NetCord.Gateway;
using TBRP.DiscordBot.Structs;

namespace TBRP.DiscordBot.Events;

public class ReadyEvent(GatewayClient client) : IEventHandler
{
    public void Register()
    {
        client.Ready += async args =>
        {
            await client.UpdatePresenceAsync(new PresenceProperties(UserStatusType.Online)
            {
                Activities =
                [
                    new UserActivityProperties(
                        "https://tbrp.site",
                        UserActivityType.Playing)
                ]
            });

            Console.WriteLine($"Logged in as {args.User.Username}");
        };
    }
}