using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using GitHubPortfolio.Models;

namespace GitHubPortfolio.Services;

public sealed class PortfolioService(HttpClient http)
{
    private const string ConfigPath = "portfolio.json";
    private const string ApiRoot = "https://api.github.com";
    private const int PageSize = 100;
    private const int MaxPages = 5; // up to 500 repositories

    public async Task<PortfolioConfig> LoadConfigAsync()
    {
        HttpResponseMessage response;
        try
        {
            response = await http.GetAsync(ConfigPath);
        }
        catch (HttpRequestException ex)
        {
            throw new PortfolioConfigException($"wwwroot/portfolio.json couldn't be downloaded ({ex.Message}).");
        }

        using (response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new PortfolioConfigException(
                    $"wwwroot/portfolio.json returned HTTP {(int)response.StatusCode}. Check that the file exists in wwwroot.");
            }

            PortfolioConfig? config;
            try
            {
                config = await response.Content.ReadFromJsonAsync(PortfolioJsonContext.Default.PortfolioConfig);
            }
            catch (JsonException ex)
            {
                throw new PortfolioConfigException(
                    $"wwwroot/portfolio.json isn't valid JSON: {ex.Message} Look for a missing comma, quote or bracket near that line.");
            }

            if (config is null)
            {
                throw new PortfolioConfigException("wwwroot/portfolio.json is empty.");
            }

            config.Normalize();
            if (config.GithubUsername.Length == 0)
            {
                throw new PortfolioConfigException("Add your GitHub username to \"githubUsername\" in wwwroot/portfolio.json.");
            }

            return config;
        }
    }

    /// <summary>Loads the profile and public repositories. Never throws; failures come back in GitHubSnapshot.Error.</summary>
    public async Task<GitHubSnapshot> LoadGitHubAsync(PortfolioConfig config)
    {
        var username = config.GithubUsername;
        var escaped = Uri.EscapeDataString(username);
        GitHubUser? user = null;

        try
        {
            using (var userResponse = await GetAsync($"{ApiRoot}/users/{escaped}"))
            {
                if (!userResponse.IsSuccessStatusCode)
                {
                    return GitHubSnapshot.Failed(DescribeFailure(userResponse, username));
                }

                user = await userResponse.Content.ReadFromJsonAsync(GitHubJsonContext.Default.GitHubUser);
            }

            var repos = new List<GitHubRepo>();
            for (var page = 1; page <= MaxPages; page++)
            {
                var url = $"{ApiRoot}/users/{escaped}/repos?type=owner&sort=pushed&per_page={PageSize}&page={page}";
                using var reposResponse = await GetAsync(url);
                if (!reposResponse.IsSuccessStatusCode)
                {
                    return GitHubSnapshot.Failed(DescribeFailure(reposResponse, username), user);
                }

                var batch = await reposResponse.Content.ReadFromJsonAsync(GitHubJsonContext.Default.ListGitHubRepo) ?? new List<GitHubRepo>();
                repos.AddRange(batch);
                if (batch.Count < PageSize)
                {
                    break;
                }
            }

            return new GitHubSnapshot(user, ApplyVisibilityRules(repos, config), null);
        }
        catch (HttpRequestException)
        {
            return GitHubSnapshot.Failed("GitHub couldn't be reached. Check your internet connection and reload the page.", user);
        }
        catch (JsonException)
        {
            return GitHubSnapshot.Failed("GitHub sent data this page couldn't read. Reload the page to try again.", user);
        }
    }

    private static List<GitHubRepo> ApplyVisibilityRules(List<GitHubRepo> repos, PortfolioConfig config)
    {
        var hidden = new HashSet<string>(config.HiddenRepos, StringComparer.OrdinalIgnoreCase);
        var featured = new HashSet<string>(config.FeaturedRepos, StringComparer.OrdinalIgnoreCase);

        return repos
            .Where(r => !hidden.Contains(r.Name))
            // Your profile README repository (username/username) isn't a project, so it's left out unless featured.
            .Where(r => featured.Contains(r.Name) || !r.Name.Equals(config.GithubUsername, StringComparison.OrdinalIgnoreCase))
            .Where(r => featured.Contains(r.Name) || config.ShowForks || !r.Fork)
            .Where(r => featured.Contains(r.Name) || config.ShowArchived || !r.Archived)
            .ToList();
    }

    private Task<HttpResponseMessage> GetAsync(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Accept.ParseAdd("application/vnd.github+json");
        return http.SendAsync(request);
    }

    private static string DescribeFailure(HttpResponseMessage response, string username)
    {
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return $"GitHub has no user called \"{username}\". Check \"githubUsername\" in wwwroot/portfolio.json.";
        }

        if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.TooManyRequests)
        {
            var resetAt = ReadRateLimitReset(response);
            return resetAt is null
                ? "GitHub's hourly request limit for this network has been reached. Reload the page later to see the repositories."
                : $"GitHub's hourly request limit for this network has been reached. Repositories load again after {resetAt.Value.ToString("HH:mm", CultureInfo.InvariantCulture)}.";
        }

        return $"GitHub returned HTTP {(int)response.StatusCode} while loading repositories. Reload the page to try again.";
    }

    private static DateTimeOffset? ReadRateLimitReset(HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues("X-RateLimit-Reset", out var values)
            && long.TryParse(values.FirstOrDefault(), out var seconds))
        {
            return DateTimeOffset.FromUnixTimeSeconds(seconds).ToLocalTime();
        }

        return null;
    }
}
