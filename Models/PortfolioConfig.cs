using GitHubPortfolio.Helpers;

namespace GitHubPortfolio.Models;

/// <summary>
/// Everything you can personalise lives in wwwroot/portfolio.json and is read into this class.
/// Property names map to camelCase JSON keys (GithubUsername -> "githubUsername").
/// </summary>
public sealed class PortfolioConfig
{
    public string GithubUsername { get; set; } = "";

    /// <summary>Overrides the name on your GitHub profile.</summary>
    public string? Name { get; set; }

    public string? Title { get; set; }

    /// <summary>One or two sentences under your name. Falls back to your GitHub bio.</summary>
    public string? Tagline { get; set; }

    public string? Email { get; set; }

    public List<ProfileLink> Links { get; set; } = [];

    public List<string> About { get; set; } = [];

    public List<SkillGroup> Skills { get; set; } = [];

    /// <summary>Repository names to show under "Selected work", in this order.
    /// Leave empty to pick your four most-starred repositories automatically.</summary>
    public List<string> FeaturedRepos { get; set; } = [];

    public List<string> HiddenRepos { get; set; } = [];

    public bool ShowForks { get; set; }

    public bool ShowArchived { get; set; } = true;

    public List<ExperienceItem> Experience { get; set; } = [];

    public string ProfileUrl => $"https://github.com/{GithubUsername}";

    public string DisplayName(GitHubUser? user) =>
        FirstFilled(Name, user?.Name, user?.Login, GithubUsername) ?? GithubUsername;

    public string? TaglineFor(GitHubUser? user) => FirstFilled(Tagline, user?.Bio);

    /// <summary>JSON can contain explicit nulls; replace them so the UI never has to check.</summary>
    public void Normalize()
    {
        GithubUsername = (GithubUsername ?? "").Trim();
        Links ??= [];
        About ??= [];
        Skills ??= [];
        FeaturedRepos ??= [];
        HiddenRepos ??= [];
        Experience ??= [];
        foreach (var group in Skills)
        {
            group.Items ??= [];
        }
    }

    /// <summary>GitHub, email, your own links, then the website from your GitHub profile if it isn't listed yet.</summary>
    public IReadOnlyList<ProfileLink> AllLinks(GitHubUser? user, bool includeEmail = true)
    {
        var links = new List<ProfileLink> { new() { Label = "GitHub", Url = ProfileUrl } };

        if (includeEmail && !string.IsNullOrWhiteSpace(Email))
        {
            links.Add(new() { Label = "Email", Url = $"mailto:{Email.Trim()}" });
        }

        links.AddRange(Links.Where(l => !string.IsNullOrWhiteSpace(l.Label) && !string.IsNullOrWhiteSpace(l.Url)));

        var website = Formatting.NormalizeUrl(user?.Blog);
        if (website is not null && !links.Any(l => SameUrl(l.Url, website)))
        {
            links.Add(new() { Label = "Website", Url = website });
        }

        return links;
    }

    private static bool SameUrl(string a, string b) =>
        string.Equals(a.Trim().TrimEnd('/'), b.Trim().TrimEnd('/'), StringComparison.OrdinalIgnoreCase);

    private static string? FirstFilled(params string?[] values) =>
        values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v))?.Trim();
}

public sealed class ProfileLink
{
    public string Label { get; set; } = "";
    public string Url { get; set; } = "";
}

public sealed class SkillGroup
{
    public string Name { get; set; } = "";
    public List<string> Items { get; set; } = [];
}

public sealed class ExperienceItem
{
    public string Role { get; set; } = "";
    public string? Organization { get; set; }
    public string? Period { get; set; }
    public string? Summary { get; set; }
}
