namespace GitHubPortfolio.Services;

/// <summary>Thrown when wwwroot/portfolio.json is missing or can't be read. The message is shown on the page.</summary>
public sealed class PortfolioConfigException(string message) : Exception(message);
