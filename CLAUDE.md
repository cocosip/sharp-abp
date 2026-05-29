# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

**sharp-abp** is an enterprise extension module collection for [ABP vNext](https://abp.io), containing 77+ framework packages and 13 business modules. Current package version: `4.7.2`. Target frameworks: `netstandard2.0`, `netstandard2.1`, `net9.0` (from `Directory.Build.props`).

## Build & Test Commands

```bash
# Restore and build the full framework solution
dotnet restore framework/SharpAbp.sln
dotnet build framework/SharpAbp.sln -c Release

# Build a specific module solution
dotnet build modules/file-storing-management/SharpAbp.Abp.FileStoringManagement.sln -c Release

# Run all tests in a solution (after build)
dotnet test framework/SharpAbp.sln --no-build -c Release

# Run a single test project
dotnet test framework/test/SharpAbp.Abp.FileStoring.Tests/SharpAbp.Abp.FileStoring.Tests.csproj -c Release

# Build everything (Windows local CI — mirrors the GitHub Actions pipeline)
powershell -ExecutionPolicy Bypass -File scripts/windows-ci/Build-All.ps1
```

The full CI solution build order is defined in `scripts/windows-ci/Build-All.ps1` and `.github/workflows/ci-cd.yml`. Both must agree.

## Repository Layout

```
framework/          – 77 infrastructure packages (src/) + xunit test projects (test/)
                      Single solution: framework/SharpAbp.sln
modules/<name>/     – 13 business modules, each with its own .sln, src/, and test/
samples/            – Runnable integration examples
scripts/windows-ci/ – Local PowerShell build scripts mirroring CI
docs/               – Per-module documentation
```

Shared MSBuild files (apply to all projects automatically via Directory.Build.props):
- `Directory.Build.props` — target frameworks, ABP/EF Core version ranges
- `Directory.Packages.props` — central NuGet version pinning (CPM)
- `common.props` — package metadata, SourceLink, lang version
- `common.test.props` — test project settings
- `configureawait.props` — Fody ConfigureAwait weaving

**Never add a `<PackageVersion>` to an individual `.csproj`** — all versions belong in `Directory.Packages.props`.

## Architecture Patterns

### ABP Module System

Every library exposes an `AbpModule` subclass (e.g., `AbpFileStoringModule`) that:
1. Declares upstream dependencies via `[DependsOn(...)]` attributes.
2. Registers services in `ConfigureServicesAsync`.

When adding a new package, follow this pattern and wire up the `[DependsOn]` chain to ensure ABP's IoC container initializes modules in order.

### File Storing Provider Pattern

The core abstraction lives in `framework/src/SharpAbp.Abp.FileStoring`. Each cloud/storage backend (Aliyun, AWS, Azure, MinIO, KS3, OBS, S3, FastDFS, FileSystem) is a separate package implementing:

| Type | Role |
|------|------|
| `IFileProvider` | Core save/get/delete/exists interface |
| `*FileProvider` | Provider-specific implementation |
| `IFileProviderSelector` | Resolves the correct provider for a container |
| `*ClientFactory` / `IObjectPool<>` | Creates/pools SDK clients (via `SharpAbp.Abp.ObjectPool`) |
| `*FileNameCalculator` | Generates storage paths |
| `*FileProviderConfiguration` | Typed configuration wrapper |
| `*FileContainerConfigurationExtensions` | Fluent API for `AbpFileStoringOptions` |

`FileContainer<T>` is the consumer-facing entry point injected via DI. It delegates to `IFileProviderSelector` → `IFileProvider`.

The `SharpAbp.Abp.ObjectPool` package provides `DefaultObjectPoolProvider`-backed pools used by provider implementations to reuse expensive SDK clients across requests.

### Business Module DDD Layers

Each module under `modules/<name>/src/` follows ABP's standard layering:

```
*.Domain.Shared     → enums, consts, DTOs shared across all layers
*.Domain            → entities, domain services, repository interfaces
*.Application.Contracts → app service interfaces, input/output DTOs, permissions
*.Application       → app service implementations, AutoMapper profiles
*.EntityFrameworkCore → EF Core DbContext, repository implementations
*.MongoDB           → MongoDB repository implementations (where provided)
*.HttpApi           → REST API controllers (thin, delegates to app services)
*.HttpApi.Client    → Typed HTTP client proxies (via ABP dynamic proxy)
```

### Multi-Target Considerations

Framework packages target `netstandard2.0;netstandard2.1;net9.0`. Avoid APIs unavailable on `netstandard2.0` unless guarded with `#if` or constrained to higher targets.

## Commit Style

Use emoji-prefixed Conventional Commits scoped to the affected package:

```
🐛 fix(file-storing): description
🏗️ build(deps): description
✨ feat(object-pool): description
```

The scope should match the package or module name (e.g., `file-storing`, `object-pool`, `kS3`).
