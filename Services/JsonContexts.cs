using System.Text.Json.Serialization;
using GitHubPortfolio.Models;

namespace GitHubPortfolio.Services;

// Source-generated JSON metadata. Blazor trims unused code when you publish,
// and source generation keeps deserialization working without reflection.

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(PortfolioConfig))]
internal partial class PortfolioJsonContext : JsonSerializerContext
{
}

[JsonSerializable(typeof(GitHubUser))]
[JsonSerializable(typeof(List<GitHubRepo>))]
internal partial class GitHubJsonContext : JsonSerializerContext
{
}
