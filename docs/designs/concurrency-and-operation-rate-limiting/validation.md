# Concurrency and Operation Rate Limiting Validation

Date: 2026-10-10

Status: In Progress. The `net9` core modules and composite operation partitions are implemented. .NET 10 compatibility, real shared-infrastructure acceptance, and `net9` to `dev` integration remain separate delivery checks.

## Operation rate limiting

The focused `net9` test run currently passes 49 tests with one opt-in Redis acceptance test skipped because `SHARP_ABP_OPERATION_REDIS` is not configured. The new coverage includes:

- single and composite built-in partitions;
- named argument matching without DTO changes;
- mixed resolver and automatic partitions;
- resolver access to multiple named arguments;
- missing named argument errors;
- legacy single-parameter resolver behavior;
- existing fixed-window, multi-rule, interception, store, and simulated shared-store tests.

Command:

```text
dotnet test framework/test/SharpAbp.Abp.OperationRateLimiting.Tests/SharpAbp.Abp.OperationRateLimiting.Tests.csproj -c Release --no-restore
```

The real Redis test remains infrastructure acceptance evidence and is not replaced by the local or simulated tests.

The same operation tests pass on isolated `.NET 10` with ABP `10.7.0`: 49 passed and one opt-in Redis test skipped. The independent concurrency tests pass on both `net9` and isolated `.NET 10` with 37 passed.

The changed operation production package builds cleanly for `netstandard2.0`, `netstandard2.1`, and `net9.0` with zero compiler warnings. The isolated `.NET 10` runs report the existing `Microsoft.Build.Tasks.Git 8.0.0` NU1902 advisory; it is unrelated to these source changes.

## Remaining checks

- Build both production packages for `netstandard2.0`, `netstandard2.1`, and `net9.0`.
- Run isolated .NET 10 compatibility tests with the latest stable ABP package version.
- Run the framework Release build and `git diff --check`.
- Integrate `net9` into `dev` only after the net9 checks are complete, then verify the .NET 10 targets on `dev`.
- Record a real two-process shared-cache/shared-lock result when the opt-in infrastructure is available.

No database-backed counter or HTTP/ASP.NET Core module is part of this validation scope.
