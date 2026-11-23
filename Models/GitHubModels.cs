using System.Text.Json.Serialization;

namespace GitHubPortfolio.Models;

// Only the fields the portfolio uses. Anything else GitHub sends is ignored.
// Docs: https://docs.github.com/rest/users/users and https://docs.github.com/rest/repos/repos

public sealed class GitHubUser
{
    [JsonPropertyName("login")] public string Login { get; set; } = "";
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("avatar_url")] public string? AvatarUrl { get; set; }
    [JsonPropertyName("html_url")] public string? HtmlUrl { get; set; }
    [JsonPropertyName("bio")] public string? Bio { get; set; }
    [JsonPropertyName("blog")] public string? Blog { get; set; }
    [JsonPropertyName("location")] public string? Location { get; set; }
    [JsonPropertyName("public_repos")] public int PublicRepos { get; set; }
    [JsonPropertyName("followers")] public int Followers { get; set; }
}

public sealed class GitHubRepo
{
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("html_url")] public string HtmlUrl { get; set; } = "";
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("homepage")] public string? Homepage { get; set; }
    [JsonPropertyName("language")] public string? Language { get; set; }
    [JsonPropertyName("topics")] public List<string>? Topics { get; set; }
    [JsonPropertyName("stargazers_count")] public int StargazersCount { get; set; }
    [JsonPropertyName("forks_count")] public int ForksCount { get; set; }
    [JsonPropertyName("fork")] public bool Fork { get; set; }
    [JsonPropertyName("archived")] public bool Archived { get; set; }
    [JsonPropertyName("pushed_at")] public DateTimeOffset? PushedAt { get; set; }
}

/// <summary>What came back from GitHub. Error is set when loading failed, in words a visitor can act on.</summary>
public sealed record GitHubSnapshot(GitHubUser? User, IReadOnlyList<GitHubRepo> Repos, string? Error)
{
    public static GitHubSnapshot Failed(string error, GitHubUser? user = null) =>
        new(user, Array.Empty<GitHubRepo>(), error);
}
