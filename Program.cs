using GitHubPortfolio;
using GitHubPortfolio.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// BaseAddress lets the app load wwwroot/portfolio.json relative to wherever it is hosted
// (for example https://you.github.io/portfolio/). GitHub API calls use absolute URLs.
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<PortfolioService>();

await builder.Build().RunAsync();
