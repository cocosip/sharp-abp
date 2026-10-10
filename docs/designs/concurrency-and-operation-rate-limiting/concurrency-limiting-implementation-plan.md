# ConcurrencyLimiting Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking. Implementation was explicitly authorized on 2026-10-10. This document does not authorize commits or branch integration.

**Date:** 2026-10-10

**Status:** In Progress; net9 core implemented and .NET 10 compatibility verified. Branch integration and real distributed acceptance are tracked separately in [validation results](validation.md).

**Goal:** Implement an independent ABP execution concurrency module on net9, using Microsoft's ConcurrencyLimiter per policy and partition.

**Architecture:** This module manages single-process execution permits without referencing OperationRateLimiting. The Manager resolves context in the current scope; a singleton registry shares native partition limiters, and leases remain held until business Tasks finish.

**Tech Stack:** .NET 9, ABP 9.3.7, System.Threading.RateLimiting, Autofac, xUnit; subsequently verify .NET 10 with the latest stable ABP 10 at implementation time.

**Spec:** [Shared module design](design.md). This plan refines files, contracts, and acceptance without expanding scope. Follow the [shared design directory conventions](../README.md).

## Global Constraints

- Implement/verify net9 first, then merge net9 into dev; the current workspace is net9.
- Production uses `$(ProjectFrameworks)`: `netstandard2.0;netstandard2.1;$(NetCoreFx)`. Tests use `$(NetCoreFx)`.
- No references to ASP.NET Core/MVC/HttpContext, caches, distributed locks, or OperationRateLimiting. No Web tests/packages or combination module.
- One permit per operation; defaults: QueueLimit=0, QueueProcessingOrder=OldestFirst, QueueTimeout=30 seconds.
- Public APIs support all TFMs; do not use .NET 9/10-only APIs for convenience.
- Exclude live limiter changes, resets of active permits, cross-process concurrency, and automatic dual-attribute composition.
- Run dotnet build/test outside the sandbox with `prefix_rule: ["dotnet"]`. Do not implicitly commit, merge, or push.

## Review Focus

- Acquisition/cancellation races must release leases not handed to business code: Task 3.
- Different scopes share one limiter; the singleton must not capture scoped services: Task 2.
- Repeated Dispose cannot create extra permits; business failure returns capacity: Task 3.
- Missing user/parameter and ordinary case-sensitive keys cannot silently allow or collide: Task 2.
- Class/method/disable precedence and proxy boundaries match documented behavior: Task 4.

## File Structure and Public Contracts

Production project: `framework/src/SharpAbp.Abp.ConcurrencyLimiting`; its `SharpAbp/Abp/ConcurrencyLimiting` source directory is abbreviated **S** below. Test project: `framework/test/SharpAbp.Abp.ConcurrencyLimiting.Tests`; its corresponding namespace directory is **T**. Use actual paths during implementation.

Namespace, AssemblyName, and PackageId correspond to `SharpAbp.Abp.ConcurrencyLimiting`.

These signatures are planned, not implemented:

```csharp
public interface IConcurrencyLimitingManager
{
    ConcurrencyLimitingLease TryAcquire(
        string policyName, ConcurrencyLimitingContext? context = null);
    Task<ConcurrencyLimitingLease> AcquireAsync(
        string policyName, ConcurrencyLimitingContext? context = null,
        CancellationToken cancellationToken = default);
    ConcurrencyLimitingStatistics GetStatistics(
        string policyName, ConcurrencyLimitingContext? context = null);
    Task ExecuteAsync(string policyName,
        Func<CancellationToken, Task> operation,
        ConcurrencyLimitingContext? context = null,
        CancellationToken cancellationToken = default);
    Task<T> ExecuteAsync<T>(string policyName,
        Func<CancellationToken, Task<T>> operation,
        ConcurrencyLimitingContext? context = null,
        CancellationToken cancellationToken = default);
}
```

| Type or Method | Contract |
| --- | --- |
| ConcurrencyLimitingContext | string? Parameter; IDictionary<string, object?> ExtraProperties |
| ConcurrencyLimitingLease : IDisposable | Read-only bool IsAcquired; idempotent Dispose; a failed lease holds no permit |
| ConcurrencyLimitingStatistics | bool IsEnabled, long AvailablePermits, long QueuedOperations; disabled values false/int.MaxValue/0 |
| ConcurrencyLimitExceededException | BusinessException; policy, PermitLimit, QueueLimit, reason (Capacity/QueueFull/QueueTimeout); no fabricated RetryAfter |
| ConcurrencyLimitingPolicy | Name, PermitLimit, QueueLimit, QueueTimeout, QueueProcessingOrder, partition definition, IsMultiTenant, ErrorCode; immutable after validation |
| IConcurrencyLimitingPolicyProvider.Get(string name) | Validated Policy; unknown name throws a configuration exception |
| SharpAbpConcurrencyLimitingOptions | IsEnabled; AddPolicy(string, Action<ConcurrencyLimitingPolicyBuilder>); ConfigurePolicy(string, Action<ConcurrencyLimitingPolicyBuilder>); both return options |
| PolicyBuilder | WithConcurrency(int permitLimit, int queueLimit=0), WithQueueTimeout(TimeSpan), WithErrorCode(string), WithMultiTenancy(bool enabled=true); return builder |
| Partition builder methods | PartitionGlobally(), PartitionByParameter(), PartitionByCurrentUser(), PartitionByCurrentTenant(), PartitionBy(string resolverName); return builder |
| Named resolver | AddPartitionKeyResolver(string, Func<IServiceProvider, ConcurrencyLimitingContext, string>); later additions replace earlier definitions with the same name |

Default partition is Global without a tenant suffix. CurrentTenant uses a stable host marker. Ordinary Parameter is not normalized; null/whitespace fails. Missing CurrentUser fails context resolution. AcquireAsync may queue; TryAcquire never does. Disabled calls do not create native limiters and return successful no-op leases, while empty names and null delegates remain invalid.

## Task 1: Policies, Configuration, and Project Dependencies

**Files:** Production/test csproj; S/SharpAbpConcurrencyLimitingModule.cs, S/SharpAbpConcurrencyLimitingOptions.cs, S/ConcurrencyLimitingPolicy.cs, S/ConcurrencyLimitingPolicyBuilder.cs, S/IConcurrencyLimitingPolicyProvider.cs, S/OptionsConcurrencyLimitingPolicyProvider.cs; T/SharpAbpConcurrencyLimitingTestModule.cs, T/SharpAbpConcurrencyLimitingTestBase.cs, T/ConcurrencyLimitingPolicyTest.cs; modify Directory.Packages.props and framework/SharpAbp.sln.

**Interfaces:** Produces Options, Policy, Builder, Provider for Tasks 2–4.

- [x] Create projects following existing Core csproj and ABP TestBase/Autofac patterns. Add only required dependencies. Select a confirmed stable 9.0.x RateLimiting version for net9; a broad range is not validation, and existing ABP is not upgraded.
- [x] Write PolicyValidationTest: reject PermitLimit=0/-1, QueueLimit=-1, QueueTimeout<=0, empty names. Same-name AddPolicy replaces; ConfigurePolicy preserves untouched fields and rejects unknown names. Changing a later builder does not mutate an already built policy.
- [x] Run focused tests and confirm missing implementation causes failure; implement AddPolicy/ConfigurePolicy/Get and final validation, then rerun successfully.
- [x] Build all TFMs with `dotnet build framework/src/SharpAbp.Abp.ConcurrencyLimiting/SharpAbp.Abp.ConcurrencyLimiting.csproj -c Release`; record resolved package versions.

## Task 2: Partition Resolution and Shared Limiters

**Files:** S/ConcurrencyLimitingContext.cs, S/ConcurrencyLimitingPartitionKey.cs, S/ConcurrencyLimitingPartitionResolver.cs, S/ConcurrencyLimiterRegistry.cs; T/ConcurrencyLimitingPartitionTest.cs, T/ConcurrencyLimiterRegistryTest.cs; module registration.

**Interfaces:** Consumes PolicyProvider; produces internal `ConcurrencyLimitingPartitionResolver.Resolve(ConcurrencyLimitingPolicy, ConcurrencyLimitingContext)` returning an immutable key, and `ConcurrencyLimiterRegistry.Get(ConcurrencyLimitingPolicy, ConcurrencyLimitingPartitionKey)` returning a shared RateLimiter.

- [x] Write SamePartitionSharesAcrossScopesTest: two scopes share policy/parameter capacity; different policies/parameters and explicitly isolated tenants remain independent.
- [x] Write PartitionIdentityTest: absent user, empty parameter, unknown resolver throw; A/a remain distinct; host is stable; same parameter shares across tenants when isolation is disabled. Custom resolver uses current-scope services without retaining that scope.
- [x] Confirm failure, then implement synchronous resolution and singleton PartitionedRateLimiter registry. Native factories capture only immutable configuration, never scoped services.
- [x] Write/pass ActivePartitionsAreNotEvictedTest and DisposeRegistryReleasesQueuedWaitersTest. Use native idle behavior without long Sleep-based synchronization; shutdown stops new acquisitions, disposes created limiters, and does not strand waiters.

## Task 3: Leases, Manager, and Async Execution

**Files:** S/IConcurrencyLimitingManager.cs, S/ConcurrencyLimitingManager.cs, S/ConcurrencyLimitingLease.cs, S/ConcurrencyLimitingStatistics.cs, S/ConcurrencyLimitExceededException.cs, S/ConcurrencyLimitingErrorCodes.cs; T/ConcurrencyLimitingManagerTest.cs.

**Interfaces:** Consumes Registry/Resolver; produces the public Manager, lease, statistics, and exception contracts.

- [x] Write PermitLifetimeTest with PermitLimit=1/QueueLimit=0: holding the first lease blocks a second, releasing allows acquisition, repeated Dispose never admits two permits. Control business entry/exit with TaskCompletionSource rather than elapsed-time inference.
- [x] Write BoundedFifoQueueTest with PermitLimit=1/QueueLimit=2: two waiters acquire in order, a third is rejected, TryAcquire never queues. ExecutionFailureReleasesPermitTest verifies Task/Task<T> failure restores capacity.
- [x] Write CancellationAndTimeoutTest: pre-cancelled token does not run a delegate; queue cancellation preserves OperationCanceledException; timeout uses a limiting exception; racing acquisition/cancellation leaves capacity=1. Repeat controlled race checks rather than treating a single passing run as exhaustive evidence.
- [x] Implement TryAcquire with AttemptAcquire(1), AcquireAsync with native AcquireAsync(1, linkedToken); distinguish failed lease, internal timeout, caller cancellation. Release acquired leases that are not delivered.
- [x] Implement ExecuteAsync using/finally lifetime and check caller cancellation again before business entry. Disabled mode creates no native instance. Statistics are snapshots and use the agreed disabled values.
- [x] Run focused tests successfully, including disable, unknown policy, lease disposal, and shutdown cases.

## Task 4: ABP Attributes, Documentation, and net9 Acceptance

**Files:** S/ConcurrencyLimitingAttribute.cs, S/DisableConcurrencyLimitingAttribute.cs, S/ConcurrencyLimitingParameterAttribute.cs, S/IHasConcurrencyLimitingParameter.cs, S/ConcurrencyLimitingInterceptor.cs, S/ConcurrencyLimitingInterceptorRegistrar.cs, S/ConcurrencyLimitingInvocationContextResolver.cs; T/ConcurrencyLimitingInterceptorTest.cs; module registration/localization; docs/ConcurrencyLimiting.md.

**Interfaces:** `IHasConcurrencyLimitingParameter.GetPartitionParameter(): string?`; interceptor consumes AcquireAsync; Registrar uses ABP OnRegistered without importing the other module for ordering.

- [x] Write actual Autofac/ABP proxy tests: class/method precedence, Disable, explicit parameter priority over DTO, ambiguous markers/DTOs rejected; virtual Task/Task<T> holds lease until business completion and releases after failure/cancellation.
- [x] Write ProxyBoundaryTest: direct new/self-call is outside automatic guarantees while manual APIs work. Unsupported synchronous/ValueTask declarative methods produce explicit registration/first-call errors, not silent protection claims.
- [x] Extract one CancellationToken argument; otherwise use None; multiple token arguments are configuration errors. Never block threads to implement asynchronous queuing.
- [x] Confirm failure, implement attribute resolution/registration/InterceptAsync try-finally, all nine FileSystem-locale resources and error-code mapping with existing ABP localization, without Web dependencies.
- [x] Document DependsOn, Configure, TryAcquire, AcquireAsync, ExecuteAsync, background jobs, attributes, permit/queue scope, and proxy boundaries.
- [x] Run focused tests, production multi-target build, `dotnet build framework/SharpAbp.sln -c Release`, and `git diff --check`. Confirm production/test direct references avoid Web packages and OperationRateLimiting.
- [ ] Record results and await the other module's net9 acceptance. Follow design Section 9 for common dev integration. Commit/merge only when authorized; do not push.

## Completion Criteria

Passing these tasks establishes net9 completion, not dev acceptance. Preserve dev's .slnx, use the latest stable ABP 10 at implementation time, and repeat multi-target production builds, tests, and framework build. Both branches expose consistent APIs/semantics.

After acceptance, update this document and the [feature index](../README.md) status and link the implemented usage documentation.
