using NetCord.Services.ApplicationCommands;

namespace TBRP.DiscordBot.Commands.Autocomplete;

public class PunishmentExpiryAutocompleteProvider : StringAutocompleteProvider
{
    private static readonly StringAutocompleteChoice[] Choices =
    [
        new("1 hour", "1h"),
        new("6 hours", "6h"),
        new("12 hours", "12h"),
        new("1 day", "1d"),
        new("3 days", "3d"),
        new("1 week", "7d"),
        new("1 month", "1m"),
        new("3 months", "3m"),
        new("1 year", "1y"),
    ];

    protected override ValueTask<IEnumerable<StringAutocompleteChoice>> GetStringChoicesAsync(
        string value,
        AutocompleteInteractionContext context)
    {
        return ValueTask.FromResult(Matching(value, Choices));
    }
}
