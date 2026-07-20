using System.Text.RegularExpressions;

namespace TBRP.DiscordBot;

partial class DiscordBotClient
{
    public static void Log(string message)
    {
        Console.WriteLine($"[{DateTimeOffset.UtcNow:O}] {message}");
    }
    
    public static DateTime? ParseDuration(string input, DateTime? from = null)
    {
        var date = from ?? DateTime.UtcNow;

        var matches = Regex.Matches(input.ToLower(), @"(\d+)([ymdh])");

        foreach (Match match in matches)
        {
            int value = int.Parse(match.Groups[1].Value);
            string unit = match.Groups[2].Value;

            date = unit switch
            {
                "y" => date.AddYears(value),
                "m" => date.AddMonths(value),
                "d" => date.AddDays(value),
                "h" => date.AddHours(value),
                _ => date
            };
        }
        
        if (matches.Count == 0 || string.Concat(matches.Select(m => m.Value)) != input.ToLower())
            return null;

        return date;
    }

    public static string? ToRelativeTimestamp(DateTime? date)
    {
        return date is null
            ? null
            : $"<t:{new DateTimeOffset(date.Value).ToUnixTimeSeconds()}:R>";
    }

    public static DateTime ParseDiscordTimestamp(string timestamp)
    {
        var match = Regex.Match(timestamp, @"<t:(\d+)(?::[tTdDfFR])?>");

        if (!match.Success)
            throw new FormatException("Invalid Discord timestamp.");

        long unix = long.Parse(match.Groups[1].Value);

        return DateTimeOffset
            .FromUnixTimeSeconds(unix)
            .UtcDateTime;
    }
}