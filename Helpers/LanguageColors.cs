namespace GitHubPortfolio.Helpers;

/// <summary>
/// The colours GitHub uses for each language (from github-linguist), so your
/// portfolio's language bar reads the same way as the bar on your repositories.
/// </summary>
public static class LanguageColors
{
    private static readonly Dictionary<string, string> Known = new(StringComparer.OrdinalIgnoreCase)
    {
        ["C#"] = "#178600",
        ["F#"] = "#b845fc",
        ["Visual Basic .NET"] = "#945db7",
        ["JavaScript"] = "#f1e05a",
        ["TypeScript"] = "#3178c6",
        ["HTML"] = "#e34c26",
        ["CSS"] = "#663399",
        ["SCSS"] = "#c6538c",
        ["Python"] = "#3572A5",
        ["Jupyter Notebook"] = "#DA5B0B",
        ["Java"] = "#b07219",
        ["Kotlin"] = "#A97BFF",
        ["Go"] = "#00ADD8",
        ["Rust"] = "#dea584",
        ["C"] = "#555555",
        ["C++"] = "#f34b7d",
        ["Objective-C"] = "#438eff",
        ["Swift"] = "#F05138",
        ["Dart"] = "#00B4AB",
        ["Ruby"] = "#701516",
        ["PHP"] = "#4F5D95",
        ["Shell"] = "#89e051",
        ["PowerShell"] = "#012456",
        ["Batchfile"] = "#C1F12E",
        ["Dockerfile"] = "#384d54",
        ["Makefile"] = "#427819",
        ["Vue"] = "#41b883",
        ["Svelte"] = "#ff3e00",
        ["Astro"] = "#ff5a03",
        ["Lua"] = "#000080",
        ["R"] = "#198CE7",
        ["Scala"] = "#c22d40",
        ["Elixir"] = "#6e4a7e",
        ["Haskell"] = "#5e5086",
        ["Zig"] = "#ec915c",
        ["Nix"] = "#7e7eff",
        ["Solidity"] = "#AA6746",
        ["Assembly"] = "#6E4C13",
        ["TeX"] = "#3D6117",
        ["GDScript"] = "#355570",
        ["ShaderLab"] = "#222c37",
        ["HLSL"] = "#aace60",
    };

    public const string Other = "#8c8fa1";

    public static string For(string? language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return Other;
        }

        if (Known.TryGetValue(language, out var hex))
        {
            return hex;
        }

        // Stable fallback colour for languages not in the list: same name, same hue, every visit.
        uint hash = 2166136261;
        foreach (var ch in language)
        {
            hash = (hash ^ ch) * 16777619;
        }

        return $"hsl({hash % 360} 55% 50%)";
    }
}
