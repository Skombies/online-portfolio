# GitHub portfolio in C#

A one-page portfolio built with C# and Blazor WebAssembly. It reads your public
repositories from the GitHub API every time someone visits, so new projects show
up without you editing anything. Everything personal lives in one file:
`wwwroot/portfolio.json`.

What's on the page:

- Your name, title, tagline, GitHub stats and links
- A language bar built from your repositories. Choosing a language filters the list below it
- Selected work: the repositories you pick, or your four most-starred
- Every public repository, with search, a language filter and sorting
- About, skills, experience and contact sections (each hides itself when empty)

It follows the visitor's light or dark mode setting.

## Run it locally

1. Install the [.NET 10 SDK](https://dotnet.microsoft.com/download).
2. In this folder, run:

   ```
   dotnet run
   ```

3. Open http://localhost:5260.

It starts out showing GitHub's mascot account, `octocat`, so you can see it working
before you change anything.

## Make it yours

Edit `wwwroot/portfolio.json`, save, and refresh the browser.

| Setting | What it does |
| --- | --- |
| `githubUsername` | Your GitHub username. The only required setting. |
| `name` | Your name. Leave as `null` to use the name on your GitHub profile. |
| `title` | Shown under your name, e.g. `"Backend developer"`. |
| `tagline` | One or two sentences under your title. Leave as `null` to use your GitHub bio. |
| `email` | Shown large in the Contact section. Use `null` to leave it out. |
| `links` | Extra links, each `{ "label": "...", "url": "..." }`. GitHub is added for you. |
| `about` | Paragraphs for the About section, one string per paragraph. |
| `skills` | Groups of skills, each `{ "name": "...", "items": ["...", "..."] }`. |
| `featuredRepos` | Repository names for Selected work, in order. Empty = your four most-starred. |
| `hiddenRepos` | Repository names to leave off the page. |
| `showForks` | `true` to include repositories you forked. |
| `showArchived` | `false` to hide archived repositories. |
| `experience` | Jobs or education, each with `role`, `organization`, `period` and `summary`. Use `[]` to hide the section. |

To link a CV, put the PDF in `wwwroot` (for example `wwwroot/cv.pdf`) and add
`{ "label": "CV", "url": "cv.pdf" }` to `links`.

Better descriptions and topics on GitHub make the page better too: the
description and topics you set on each repository are what visitors read here.

## Put it online with GitHub Pages

1. Create a GitHub repository and push this folder to its `main` branch.
   - Name it `<your-username>.github.io` to get the address `https://<your-username>.github.io`.
   - Any other name works too; the address becomes `https://<your-username>.github.io/<repo-name>/`.
2. In the repository, go to **Settings > Pages** and set **Source** to **GitHub Actions**.
3. Push again (or run the workflow from the **Actions** tab). The included workflow
   builds the site, sets the right base path for your address, and publishes it.

## How it's put together

```
Program.cs                    App startup and services
App.razor                     Router
Pages/Home.razor              The page: loads settings and GitHub data, lays out sections
Components/                   Masthead, LanguageSpectrum, FeaturedProject, RepoList, ...
Services/PortfolioService.cs  Reads portfolio.json and calls the GitHub API
Models/                       Settings and GitHub data classes
Helpers/                      Number/date formatting, language colours and counts
wwwroot/portfolio.json        Your content
wwwroot/css/app.css           All styles (colours are variables at the top)
.github/workflows/deploy.yml  Build and publish to GitHub Pages
```

## Good to know

- GitHub allows 60 unauthenticated API requests per hour per visitor's network.
  A visit uses about two, so normal visitors never notice. If you refresh a lot while
  testing, the page tells you when the limit resets.
- The language bar counts each repository's main language, as GitHub reports it.
- Colours, spacing and type sizes are CSS variables at the top of `wwwroot/css/app.css`.
