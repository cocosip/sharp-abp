# Concurrency and Operation Rate Limiting Validation

Date: 2026-10-10

Status: In Progress. The `net9` core modules and composite operation partitions are implemented, and the review fixes are applied locally. Current test/build execution is blocked during NuGet restore by the environment's TLS/credential failure for `https://api.nuget.org/v3/index.json`. .NET 10 compatibility, real shared-infrastructure acceptance, and `net9` to `dev` integration remain separate delivery checks.

## Operation rate limiting

Before the review-fix changes, the focused `net9` test run recorded 49 passing tests with one opt-in Redis acceptance test skipped because `SHARP_ABP_OPERATION_REDIS` was not configured. The review adds coverage for:

- single and composite built-in partitions;
- named argument matching without DTO changes;
- mixed resolver and automatic partitions;
- resolver access to multiple named arguments;
- missing named argument errors;
- legacy single-parameter resolver behavior;
- existing fixed-window, multi-rule, interception, store, and simulated shared-store tests.
- interface-level concurrency attributes;
- unsupported composite argument objects and malformed expired cache state;
- immediate configuration delegate validation;
- single-rule consume fast path and concurrent fixed-window prechecks.

Command:

```text
dotnet test framework/test/SharpAbp.Abp.OperationRateLimiting.Tests/SharpAbp.Abp.OperationRateLimiting.Tests.csproj -c Release --no-restore
```

The real Redis test remains infrastructure acceptance evidence and is not replaced by the local or simulated tests.

The previous isolated `.NET 10` results are historical evidence for the pre-review code; they do not validate the current changes. The current environment has not produced a fresh test result because restore is blocked.

The changed operation production package builds cleanly for `netstandard2.0`, `netstandard2.1`, and `net9.0` with zero compiler warnings. The isolated `.NET 10` runs report the existing `Microsoft.Build.Tasks.Git 8.0.0` NU1902 advisory; it is unrelated to these source changes.

## Remaining checks

- Build both production packages for `netstandard2.0`, `netstandard2.1`, and `net9.0`.
- Run isolated .NET 10 compatibility tests with the latest stable ABP package version.
- Run the framework Release build and `git diff --check`.
- Integrate `net9` into `dev` only after the net9 checks are complete, then verify the .NET 10 targets on `dev`.
- Record a real two-process shared-cache/shared-lock result when the opt-in infrastructure is available.

No database-backed counter or HTTP/ASP.NET Core module is part of this validation scope.
