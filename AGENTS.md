# Repository Guidelines

## Project Structure & Module Organization

This repository is a .NET/ABP package collection. Shared infrastructure packages live under `framework/src`, with framework tests in `framework/test` and the main framework solution at `framework/SharpAbp.sln`. Business modules live under `modules/<module-name>` and usually contain their own `.sln`, `src`, and `test` trees. Runnable examples and integration samples are under `samples/`. Shared build settings are in `Directory.Build.props`, `Directory.Packages.props`, `common.props`, and `common.test.props`.

## Build, Test, and Development Commands

- `dotnet restore framework/SharpAbp.sln` restores the framework solution.
- `dotnet build framework/SharpAbp.sln -c Release` builds the shared framework packages.
- `dotnet test framework/test/SharpAbp.Abp.FileStoring.Tests/SharpAbp.Abp.FileStoring.Tests.csproj -c Release` runs a focused test project.
- `dotnet build modules/file-storing-management/SharpAbp.Abp.FileStoringManagement.sln -c Release` builds one module solution.
- `powershell -ExecutionPolicy Bypass -File scripts/windows-ci/Build-All.ps1` builds the framework and module solution list used by local Windows CI.

In the Codex app, run `dotnet build` and `dotnet test` outside the sandbox with the approved `dotnet` prefix rule.

## Coding Style & Naming Conventions

Use C# with the shared `LangVersion` from `common.props` and `common.test.props`. Follow existing ABP conventions: PascalCase for public types and members, camelCase for locals and parameters, async method names ending in `Async`, and namespaces matching the `SharpAbp.*` project path. Keep indentation consistent with nearby files. Central package versions belong in `Directory.Packages.props`; project-wide MSBuild behavior belongs in the shared props files, not individual projects unless scoped.

## Testing Guidelines

Tests are ordinary .NET test projects under `framework/test` and `modules/*/test`, commonly named `*.Tests` or `*.TestBase`. Add regression coverage beside the affected package or module, and prefer focused runs before broader solution builds. Test classes typically use names such as `SomeServiceTest` or `SomeModule_Tests`; follow the local project pattern.

## Commit & Pull Request Guidelines

Recent history often uses emoji-prefixed Conventional Commit subjects, with scopes such as `fix(file-storing): ...` and `build(deps): ...`. Keep commits scoped, imperative, and tied to the affected package or module. Pull requests should state the problem, summarize the fix, list the exact build/test commands run, and call out configuration or package-version changes.

## Security & Configuration Tips

Do not commit credentials, local endpoints, or machine-specific secrets. Keep sample configuration generic and document required environment-specific values in the relevant sample or module README.

## Branch, Version & Release Policy

Three long-lived branches, each pinned to one dependency baseline:

| Branch | Baseline | Role | Package upgrades |
| --- | --- | --- | --- |
| `master` | .NET 10 / ABP 10.x | Main line; receives merges from `dev`; releases are tagged here | through `dev` only |
| `dev` | .NET 10 / ABP 10.x | Primary feature development | the only branch that upgrades packages |
| `net9` | .NET 9 / ABP 9.3.x | Maintenance only: bug and security fixes for the `4.7.x` line | frozen |

- Feature work happens on `dev` only and then merges into `master`.
- A bug fixed on `net9` must also reach `dev` when the affected feature exists there and is expected to behave the same, so shared features behave identically on both lines. Land it as a real merge of the `net9` commit into `dev`, not a cherry-pick or a re-implementation; when the migration already rewrote the file, resolve the ABP 9 vs ABP 10 differences in the merge commit and run the affected tests on `dev`. `dev`-only fixes are not back-ported to `net9` unless they close a security issue.
- Never upgrade packages on `net9`: keep `Directory.Build.props`, `Directory.Packages.props`, and third-party SDK versions as they are. Exceptions are limited to security advisories or hard blockers and must be stated in the commit, for example `net9: pin <package> <reason>`.
- Adopt a new .NET or ABP major on `dev` by moving `NetCoreFx` and `VoloAbpPackageVersion` in `Directory.Build.props`. Do not open a new branch per .NET version.
- `common.props` `<Version>` is the single version source: `4.7.x` on `net9`, `6.x` on `dev`/`master`. A release tag `v<Version>` must equal it and point at the built commit; CI fails before building otherwise.

CI/CD conventions in `.github/workflows/ci-cd.yml`:

- CI runs for `master`, `dev`, `net9`, and `v*` tags; other branches do not trigger it.
- A tag push packs every solution and publishes to nuget.org through **NuGet Trusted Publishing**: `NuGet/login@v1` exchanges the GitHub OIDC token for a one-hour key, so the `id-token: write` permission is required and no long-lived `NUGET_API_KEY` secret may be added. nuget.org needs a trusted publishing policy for `cocosip/sharp-abp` + `ci-cd.yml`.
- Keep the solution list in the workflow and in `scripts/windows-ci/Build-All.ps1` in sync.
