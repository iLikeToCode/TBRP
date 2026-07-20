using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.ComponentInteractions;
using TBRP.DB;
using TBRP.Api;
using TBRP.DiscordBot.Commands.Autocomplete;

namespace TBRP.DiscordBot.Commands.SlashCommands.ErlcCommand;

partial class ErlcCommand
{
    partial class PunishmentCommand
    {
        [SubSlashCommand("create", "Create a punishment record")]
        public async Task Create(PunishmentType type,
            [SlashCommandParameter(AutocompleteProviderType = typeof(IngameRobloxUserAutocompleteProvider))]
            string robloxUser,
            string reason,
            [SlashCommandParameter(AutocompleteProviderType = typeof(PunishmentExpiryAutocompleteProvider))]
            string? expiry = "1d")
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

                if (type == PunishmentType.Ban && !guildUser.RoleIds.Contains(DiscordBotClient.BanPermsId))
                {
                    await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
                    {
                        Content = "You do not have permission to ban a user.",
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

            if (reason.Length < 3)
            {
                await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
                {
                    Content = "Invalid Reason.",
                    Flags = MessageFlags.Ephemeral
                }));
                return;
            }

            var expiryDate = DiscordBotClient.ParseDuration(expiry ?? "1d");
            if (expiryDate == null && expiry != null)
            {
                await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
                {
                    Content = "Invalid Expiry (Format: 1d or 1m or 1y).",
                    Flags = MessageFlags.Ephemeral
                }));
                return;
            }

            await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties()
            {
                Embeds =
                [
                    new EmbedProperties()
                    {
                        Title = "Punishment Creation",
                        Color = new Color(255, 170, 51),
                        Fields = [
                            new EmbedFieldProperties()
                            {
                                Name = "Type",
                                Value = type.ToString()
                            },
                            new EmbedFieldProperties()
                            {
                                Name = "User",
                                Value = rblxUser.Name
                            },
                            new EmbedFieldProperties()
                            {
                                Name = "Reason",
                                Value = reason
                            },
                            new EmbedFieldProperties()
                            {
                                Name = "Expiry",
                                Value = DiscordBotClient.ToRelativeTimestamp(expiryDate) ?? "Never"
                            },
                            new EmbedFieldProperties()
                            {
                                Name = "Issuer",
                                Value = $"{Context.User.Id}"
                            }
                        ]
                    }
                ],
                Components = [
                    new ActionRowProperties().AddComponents([
                            new ButtonProperties("erlc_punishment_create_confirm",
                                "Confirm", ButtonStyle.Success),
                            new ButtonProperties("erlc_punishment_create_cancel",
                            "Cancel", ButtonStyle.Danger)
                        ]
                        )
                ]
            }));
        }
    }
}

public class Erlc_Punishment_Create_Buttons (ApiClient apiClient) : ComponentInteractionModule<ButtonInteractionContext>
{
    [ComponentInteraction("erlc_punishment_create_confirm")]
    public async Task Confirm()
    {
        var fields = Context.Message.Embeds[0].Fields;

        PunishmentType type;
        PunishmentType.TryParse(fields.First(m => m.Name == "Type").Value, out type);
        
        var expiry = fields.First(m => m.Name == "Expiry");
        var expiryDate = expiry.Value == "None" ? (DateTime?)null : DiscordBotClient.ParseDiscordTimestamp(expiry.Value);

        var robloxUserName = fields.First(m => m.Name == "User").Value;
        
        var reason = fields.First(m => m.Name == "Reason").Value;
        
        var issuer = fields.First(m => m.Name == "Issuer").Value;

        if (Context.User.Id.ToString() != issuer)
        {
            await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
            {
                Content = "This is not your punishment.",
                Flags = MessageFlags.Ephemeral
            }));
            return;
        }
        
        await using var ctx = new TbrpContext();

        if (type == PunishmentType.Warn)
        {
            ctx.Punishments.Add(new Punishment()
            {
                Type = type,
                Reason = reason,
                Expiry = expiryDate,
                CreatorId = Context.User.Id,
                ActionTaken = false
            });
        }
        else
        {
            if (type == PunishmentType.Kick)
            {
                await apiClient.ErlcV2.SendCommand($":kick {robloxUserName}");
                ctx.Punishments.Add(new Punishment()
                {
                    Type = type,
                    Reason = reason,
                    Expiry = expiryDate,
                    CreatorId = Context.User.Id,
                    ActionTaken = false
                });
            } else if (type == PunishmentType.Ban)
            {
                var users = await apiClient.RobloxUserV1.GetUsersByUsername(robloxUserName);
                if (users is null || users.Length < 1) return;
                var id = users[0].Id;
                await apiClient.ErlcV2.SendCommand($":ban {id}");
                ctx.Punishments.Add(new Punishment()
                {
                    Type = type,
                    Reason = reason,
                    Expiry = expiryDate,
                    CreatorId = Context.User.Id,
                    ActionTaken = false
                });
            }
        }

        try
        {
            await ctx.SaveChangesAsync();
            await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties()
            {
                Embeds = [
                new EmbedProperties()
                {
                    Title = "Punishment Confirmed",
                    Description = "Punishment has been successfully stored and any necessary action has been or will be taken.",
                    Color = new Color(0, 128, 0)
                }]
            }));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await Context.Interaction.SendResponseAsync(InteractionCallback.Message("Database Error. <@844951775106433024>"));
        }
    }
}
