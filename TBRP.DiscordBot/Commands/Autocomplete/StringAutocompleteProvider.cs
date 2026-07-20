using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace TBRP.DiscordBot.Commands.Autocomplete;

public abstract class StringAutocompleteProvider : IAutocompleteProvider<AutocompleteInteractionContext>
{
    private const int MaxChoices = 25;
    private const int MaxChoiceLength = 100;

    public async ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(
        ApplicationCommandInteractionDataOption option,
        AutocompleteInteractionContext context)
    {
        var value = option.Value?.Trim() ?? string.Empty;
        var choices = await GetStringChoicesAsync(value, context);

        return choices
            .Where(choice => !string.IsNullOrWhiteSpace(choice.Name)
                             && !string.IsNullOrWhiteSpace(choice.Value))
            .Select(choice => new ApplicationCommandOptionChoiceProperties(
                Truncate(choice.Name.Trim()),
                Truncate(choice.Value.Trim())))
            .Take(MaxChoices);
    }

    protected abstract ValueTask<IEnumerable<StringAutocompleteChoice>> GetStringChoicesAsync(
        string value,
        AutocompleteInteractionContext context);

    protected static IEnumerable<StringAutocompleteChoice> Matching(
        string value,
        IEnumerable<StringAutocompleteChoice> choices)
    {
        if (value.Length == 0)
        {
            return choices;
        }

        return choices.Where(choice =>
            choice.Name.Contains(value, StringComparison.OrdinalIgnoreCase)
            || choice.Value.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    private static string Truncate(string value)
    {
        return value.Length <= MaxChoiceLength ? value : value[..MaxChoiceLength];
    }
}

public readonly record struct StringAutocompleteChoice(string Name, string Value);
