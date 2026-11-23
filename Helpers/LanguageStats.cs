using GitHubPortfolio.Models;

namespace GitHubPortfolio.Helpers;

/// <param name="Count">How many repositories have this as their main language.</param>
/// <param name="IsOther">True for the bucket that groups the less-used languages.</param>
public sealed record LanguageStat(string Name, int Count, string Color, bool IsOther = false);

public static class LanguageStats
{
    /// <summary>
    /// Counts repositories by main language, most used first. Past <paramref name="maxShown"/>
    /// languages, the rest are grouped so the bar stays readable.
    /// </summary>
    public static IReadOnlyList<LanguageStat> From(IEnumerable<GitHubRepo> repos, int maxShown = 10)
    {
        var stats = repos
            .Where(r => !string.IsNullOrWhiteSpace(r.Language))
            .GroupBy(r => r.Language!, StringComparer.OrdinalIgnoreCase)
            .Select(g => new LanguageStat(g.Key, g.Count(), LanguageColors.For(g.Key)))
            .OrderByDescending(s => s.Count)
            .ThenBy(s => s.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (stats.Count <= maxShown)
        {
            return stats;
        }

        var shown = stats.Take(maxShown - 1).ToList();
        var rest = stats.Skip(maxShown - 1).ToList();
        shown.Add(new LanguageStat($"{rest.Count} more languages", rest.Sum(s => s.Count), LanguageColors.Other, IsOther: true));
        return shown;
    }
}
