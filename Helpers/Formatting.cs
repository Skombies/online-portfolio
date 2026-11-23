using System.Globalization;

namespace GitHubPortfolio.Helpers;

public static class Formatting
{
    public static string Count(int value) => value.ToString("N0", CultureInfo.InvariantCulture);

    public static string Plural(int value, string singular, string plural) =>
        $"{Count(value)} {(value == 1 ? singular : plural)}";

    public static string Updated(DateTimeOffset? when)
    {
        if (when is null)
        {
            return "No commits yet";
        }

        var days = (int)Math.Floor((DateTimeOffset.UtcNow - when.Value).TotalDays);
        return days switch
        {
            < 1 => "Updated today",
            1 => "Updated yesterday",
            < 30 => $"Updated {days} days ago",
            _ => $"Updated {when.Value.ToString("MMM yyyy", CultureInfo.InvariantCulture)}",
        };
    }

    /// <summary>"a", "a and b", "a, b and c"</summary>
    public static string JoinWithAnd(IReadOnlyList<string> parts) => parts.Count switch
    {
        0 => "",
        1 => parts[0],
        _ => string.Join(", ", parts.Take(parts.Count - 1)) + " and " + parts[^1],
    };

    /// <summary>GitHub lets people type "example.com" as their website; links need a scheme.</summary>
    public static string? NormalizeUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        url = url.Trim();
        return url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            ? url
            : "https://" + url;
    }

    /// <summary>
    /// True for links to files in your own site, such as "cv.pdf". Those open in a new tab
    /// so the Blazor router doesn't try to treat them as a page.
    /// </summary>
    public static bool IsLocalFile(string url) =>
        !url.Contains(':') && !url.StartsWith('#') && !url.StartsWith("//");
}
