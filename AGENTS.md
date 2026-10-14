# Repository Guidelines

## Project Structure & Module Organization

SharpAbp is a .NET/ABP package collection. Infrastructure packages and tests live in `framework/src` and `framework/test`; the solution is `framework/SharpAbp.slnx`. Business modules use `modules/<name>/src`, `test`, and individual `.slnx` solutions. Runnable examples and web assets live under `samples`; documentation belongs in `docs` or module READMEs.

Shared settings live in `Directory.Build.props`, `Directory.Packages.props`, `common.props`, and `common.test.props`. The current checkout uses .NET 10 and ABP 10.x; some infrastructure packages also target .NET Standard. Check each project's targets before using framework-specific APIs.

## Build, Test, and Development Commands

Run from the repository root with the .NET 10 SDK:

- `dotnet restore framework/SharpAbp.slnx` — restore framework dependencies.
- `dotnet build framework/SharpAbp.slnx -c Release` — build infrastructure packages.
- `dotnet build modules/account/SharpAbp.Abp.Account.slnx -c Release` — build one module.
- `dotnet test modules/account/test/SharpAbp.Abp.Account.Application.Tests/SharpAbp.Abp.Account.Application.Tests.csproj -c Release` — run focused tests.
- `dotnet run --project samples/sample/src/SharpSample.HttpApi.Host` — start the integration sample after configuring its external services.
- `powershell -ExecutionPolicy Bypass -File scripts/windows-ci/Build-All.ps1` — build the framework and module CI list.
- `git diff --check` — check whitespace errors.

Keep `Build-All.ps1` aligned with `.github/workflows/ci-cd.yml`; samples require separate validation.

## Coding Style & Naming Conventions

Follow nearby formatting: C# typically uses four spaces. Use PascalCase for public members, camelCase for locals, `Async` suffixes, and `SharpAbp.*` namespaces. Respect shared `LangVersion` settings. Centralize NuGet versions in `Directory.Packages.props`; omit inline package versions.

Use Mapperly, never AutoMapper. Follow existing `[Mapper]`, `MapperBase`, and `AddMapperlyObjectMapper<TModule>()` patterns. Use `[MapExtraProperties]` for extensible pairs and preserve ABP definition checks for nested mappings. Preserve DTO contracts and protected extension points.

## Testing Guidelines

Tests use xUnit and existing ABP test bases; follow names such as `SomeServiceTest` or `SomeModule_Tests`. Add focused regression coverage beside the affected module. Mapping tests should cover both overloads, nested fields, and extension properties. Run focused tests before broader builds; report warnings and validation limits.


## Agent Execution & Security

In Codex, run `dotnet build` and `dotnet test` outside the sandbox with `sandbox_permissions: "require_escalated"` and `prefix_rule: ["dotnet"]`. Reuse approved permissions; do not repeatedly retry sandbox hangs. Build scripts need equivalent execution access.

Never commit credentials or machine-specific endpoints; keep sample configuration generic.
