# Repository Guidelines

## Project Structure & Module Organization
GameSpace runs as an ASP.NET MVC solution anchored at `GameSpace/GameSpace.sln`. The primary web app lives at `GameSpace/GameSpace/GameSpace` with feature areas under `Areas/Forum`, `Areas/MemberManagement`, `Areas/MiniGame`, `Areas/OnlineStore`, and `Areas/SocialHub`. Shared controllers and Razor views belong in `GameSpace/GameSpace/GameSpace/Controllers` and `.../Views`. Place data access in `GameSpace/GameSpace/GameSpace/Data`, domain models in `.../Models`, helpers and policies in `.../Infrastructure`, and static assets in `.../wwwroot`. Tests reside in `GameSpace/GameSpace.Tests`. Consult `schema/` before adjusting persistent storage.

## Build, Test, and Development Commands
- `dotnet restore GameSpace/GameSpace.sln` hydrates NuGet packages for all projects.
- `dotnet build GameSpace/GameSpace.sln -c Debug` validates compilation prior to opening a PR.
- `dotnet watch run --project GameSpace/GameSpace/GameSpace.csproj` starts the MVC app with hot reload.
- `libman restore` (run inside `GameSpace/GameSpace/GameSpace`) refreshes client-side libraries.
- `dotnet format GameSpace/GameSpace.sln` enforces repository formatting rules.

## Coding Style & Naming Conventions
Target .NET 8 with nullable reference types enabled. Use Allman braces, four-space indentation, and order `using` directives: framework → third-party → project. Adopt PascalCase for types/methods, camelCase for locals and parameters, and `_camelCase` for private fields. Keep Razor files idiomatic and avoid inline scripts when a static asset fits better.

## Testing Guidelines
All automated tests live in `GameSpace/GameSpace.Tests` and follow xUnit. Name classes `FeatureNameTests` and tests in the `Scenario_Should_ExpectedResult` pattern. Run `dotnet test` before submitting changes and highlight coverage gaps for critical paths or security-sensitive code.

## Commit & Pull Request Guidelines
Write imperative commit messages, optionally scoped (e.g., `fix(Forum): adjust thread paging`). Pull requests must summarize intent, list validation steps such as `dotnet build` and `dotnet test`, and call out schema or configuration updates. Include UI screenshots when relevant and link associated work items. Request reviewers from affected feature areas or infrastructure owners when touching cross-cutting code.

## Security & Configuration Tips
Never commit secrets; store local credentials with `dotnet user-secrets` and use environment variables in CI. Develop under `ASPNETCORE_ENVIRONMENT=Development` and document any new keys added to `appsettings*.json`. Surface planned authorization or policy changes early so security reviewers can validate elevated paths.
