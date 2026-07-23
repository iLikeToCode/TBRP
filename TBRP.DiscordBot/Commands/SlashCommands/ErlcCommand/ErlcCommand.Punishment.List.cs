using System.Text;
using Microsoft.EntityFrameworkCore;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.ComponentInteractions;
using TBRP.DB;
using TBRP.DiscordBot.Commands.Autocomplete;

namespace TBRP.DiscordBot.Commands.SlashCommands.ErlcCommand;

partial class ErlcCommand
{
    partial class PunishmentCommand
    {
        [SubSlashCommand("list", "List punishment records for a user")]
        public async Task List(
            [SlashCommandParameter(AutocompleteProviderType = typeof(IngameRobloxUserAutocompleteProvider))]
            string robloxUser)
        {
            if (Context.User is GuildUser guildUser)
            {
                if (!guildUser.RoleIds.Contains(DiscordBotClient.StaffRoleId))
                {
                    await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
                    {
                        Content = "You do not have permission to run this command.",
                        Flags = MessageFlags.Ephemeral
                    }));
                    return;
                }
            }
            
            var users = await apiClient.RobloxUserV1.GetUsersByUsername(robloxUser);
            if (users == null || users.Length < 1)
            {
                await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
                {
                    Content = "No roblox user found.",
                    Flags = MessageFlags.Ephemeral
                }));
                return;
            }

            var rblxUser = users[0];

            var message = await Erlc_Punishment_List_Buttons.BuildMessageAsync(rblxUser.Id, rblxUser.Name, Context.User.Id, 0);

            await Context.Interaction.SendResponseAsync(InteractionCallback.Message(message));
        }
    }
}

public class Erlc_Punishment_List_Buttons : ComponentInteractionModule<ButtonInteractionContext>
{
    private const int PageSize = 5;

    [ComponentInteraction("erlc_punishment_list_prev")]
    public Task Previous(long robloxId, ulong requesterId, int page)
    {
        return ChangePage(robloxId, requesterId, page);
    }

    [ComponentInteraction("erlc_punishment_list_next")]
    public Task Next(long robloxId, ulong requesterId, int page)
    {
        return ChangePage(robloxId, requesterId, page);
    }

    public static async Task<InteractionMessageProperties> BuildMessageAsync(
        long robloxId,
        string robloxUserName,
        ulong requesterId,
        int page)
    {
        await using var ctx = new TbrpContext();

        var totalPunishments = await ctx.Punishments
            .Where(p => p.RobloxId == robloxId)
            .CountAsync();

        var totalPages = Math.Max(1, (int)Math.Ceiling(totalPunishments / (double)PageSize));
        page = Math.Clamp(page, 0, totalPages - 1);

        var punishments = await ctx.Punishments
            .Where(p => p.RobloxId == robloxId)
            .OrderByDescending(p => p.CreatedDate)
            .ThenByDescending(p => p.Id)
            .Skip(page * PageSize)
            .Take(PageSize)
            .ToListAsync();

        var start = totalPunishments == 0 ? 0 : page * PageSize + 1;
        var end = Math.Min((page + 1) * PageSize, totalPunishments);

        var embed = new EmbedProperties
        {
            Title = $"Punishments for {robloxUserName}",
            Description = totalPunishments == 0
                ? "No punishment records found."
                : $"Showing {start}-{end} of {totalPunishments} punishment records.",
            Color = new Color(255, 170, 51),
            Fields = punishments.Select(BuildPunishmentField).ToArray()
        };

        return new InteractionMessageProperties
        {
            Embeds = [embed],
            Components = BuildComponents(robloxId, requesterId, page, totalPages)
        };
    }

    private async Task ChangePage(long robloxId, ulong requesterId, int page)
    {
        if (Context.User.Id != requesterId)
        {
            await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
            {
                Content = "This is not your punishment list.",
                Flags = MessageFlags.Ephemeral
            }));
            return;
        }

        const string titlePrefix = "Punishments for ";
        var title = Context.Message.Embeds[0].Title ?? "";
        var robloxUserName = title.StartsWith(titlePrefix)
            ? title[titlePrefix.Length..]
            : robloxId.ToString();
        var message = await BuildMessageAsync(robloxId, robloxUserName, requesterId, page);

        await Context.Interaction.SendResponseAsync(InteractionCallback.ModifyMessage(m =>
        {
            m.WithEmbeds(message.Embeds);
            m.WithComponents(message.Components);
        }));
    }

    private static EmbedFieldProperties BuildPunishmentField(Punishment punishment)
    {
        var created = ToDiscordTimestamp(punishment.CreatedDate, "f") ?? "Unknown";
        var expiry = ToDiscordTimestamp(punishment.Expiry, "R") ?? "Never";

        var value = new StringBuilder()
            .AppendLine($"ID: {punishment.Id}")
            .AppendLine($"Reason: {Truncate(punishment.Reason ?? "No reason recorded.", 600)}")
            .AppendLine($"Issuer: <@{punishment.CreatorId}>")
            .AppendLine($"Created: {created}")
            .AppendLine($"Expiry: {expiry}")
            .Append($"Action Taken: {(punishment.ActionTaken ? "Yes" : "No")}")
            .ToString();

        return new EmbedFieldProperties
        {
            Name = $"#{punishment.Id} - {punishment.Type}",
            Value = value,
            Inline = false
        };
    }

    private static IMessageComponentProperties[] BuildComponents(
        long robloxId,
        ulong requesterId,
        int page,
        int totalPages)
    {
        return
        [
            new ActionRowProperties().AddComponents(
            [
                new ButtonProperties(
                    $"erlc_punishment_list_prev:{robloxId}:{requesterId}:{page - 1}",
                    "Previous",
                    ButtonStyle.Secondary).WithDisabled(page <= 0),
                new ButtonProperties(
                    "erlc_punishment_list_page",
                    $"Page {page + 1}/{totalPages}",
                    ButtonStyle.Primary).WithDisabled(true),
                new ButtonProperties(
                    $"erlc_punishment_list_next:{robloxId}:{requesterId}:{page + 1}",
                    "Next",
                    ButtonStyle.Secondary).WithDisabled(page >= totalPages - 1)
            ])
        ];
    }

    private static string? ToDiscordTimestamp(DateTime? date, string style)
    {
        return date is null
            ? null
            : $"<t:{new DateTimeOffset(DateTime.SpecifyKind(date.Value, DateTimeKind.Utc)).ToUnixTimeSeconds()}:{style}>";
    }

    private static string Truncate(string value, int maxLength)
    {
        return value.Length <= maxLength
            ? value
            : $"{value[..(maxLength - 3)]}...";
    }
}
