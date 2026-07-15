using System.Text;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace TBRP.DiscordBot.Commands.SlashCommands.ERLC;

partial class ErlcCommands
{
    [SubSlashCommand("status", "Fetch in-game server status.")]
    public async Task<InteractionMessageProperties> Status()
    {
        var callback = InteractionCallback.DeferredMessage();
        var status = await apiClient.V1.GetStatus();

        var coOwners = new StringBuilder();
        foreach (var user in await apiClient.RobloxUserV1.GetUsersByIds(status.CoOwners))
        {
            coOwners.AppendLine($"[{user.DisplayName} ({user.Name})](<https://roblox.com/users/{user.Id}>)");
        }

        var owner = await apiClient.RobloxUserV1.GetUserById(status.OwnerId);

        var embed = new EmbedProperties()
        {
            Title = status.ServerName,
            Color = new Color(255, 255, 255),
            Fields =
            [
                new EmbedFieldProperties()
                {
                    Name = "Active Players",
                    Value = $"{status.PlayerCount}/{status.MaxPlayers}",
                    Inline = true
                },
                new EmbedFieldProperties()
                {
                    Name = "Join Code",
                    Value = status.JoinCode,
                    Inline = true
                },
                new EmbedFieldProperties()
                {
                    Name = "\u200b",
                    Value = "\u200b",
                    Inline = true
                },
                new EmbedFieldProperties()
                {
                    Name = "Owner",
                    Value = $"[{owner.DisplayName} ({owner.Name})](<https://roblox.com/users/{owner.Id}>)",
                    Inline = true
                },
                new EmbedFieldProperties()
                {
                    Name = "Co-Owners",
                    Value = coOwners.ToString(),
                    Inline = true
                },
                new EmbedFieldProperties()
                {
                    Name = "\u200b",
                    Value = "\u200b",
                    Inline = true
                },
                new EmbedFieldProperties()
                {
                    Name = "Team Balance",
                    Value = status.TeamBalance ? "Enabled" : "Disabled",
                    Inline = true
                },
                new EmbedFieldProperties()
                {
                    Name = "Account Verification",
                    Value = status.AccountVerificationRequirement,
                    Inline = true
                },
                new EmbedFieldProperties()
                {
                    Name = "\u200b",
                    Value = "\u200b",
                    Inline = true
                }
            ]
        };

        return new InteractionMessageProperties().AddEmbeds(embed);
    }
}