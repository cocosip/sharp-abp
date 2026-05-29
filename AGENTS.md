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
