using NetCord.Services.ApplicationCommands;
using TBRP.Api;

namespace TBRP.DiscordBot.Commands.Autocomplete;

public class IngameRobloxUserAutocompleteProvider(ApiClient apiClient) : StringAutocompleteProvider
{
    protected override async ValueTask<IEnumerable<StringAutocompleteChoice>> GetStringChoicesAsync(
        string value,
        AutocompleteInteractionContext context)
    {
        return [];
        if (value.Length < 3)
        {
            return [];
        }

        var users = await apiClient.RobloxUserV1.SearchUsersByUsername(value);
        if (users.Length == 0)
        {
            return [];
        }

        return users.Select(user =>
        {
            var displayName = string.Equals(user.Name, user.DisplayName, StringComparison.OrdinalIgnoreCase)
                ? user.Name
                : $"{user.DisplayName} (@{user.Name})";

            return new StringAutocompleteChoice($"{displayName} ({user.Id})", user.Name);
        });
    }
}
