# OperationRateLimiting Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking. Implementation was explicitly authorized on 2026-10-10. This document does not authorize commits or branch integration.

**Date:** 2026-10-10

**Status:** In Progress; net9 core and composite-partition changes are implemented and focused tests are passing. All-TFM compatibility, real distributed acceptance, and dev integration remain tracked separately in [validation results](validation.md).

**Goal:** Implement a general operation-count limiting module on net9, similar to ABP Pro, with policies, single/composite partitions, named resolver extensions, and a replaceable Store.

**Architecture:** Checker supplies manual/service-attribute entry points; each Rule resolves one or more named partition values and consumes Store results. Direct partitions handle ordinary values and ABP identities; named resolvers handle derived or DI-backed values. Store owns per-key atomic consumption/reset, using typed ABP cache/lock abstractions by default. Checker neither holds locks nor references ConcurrencyLimiting.

**Tech Stack:** .NET 9, ABP 9.3.7 cache/lock abstractions/identity/tenants/clock, Autofac, xUnit; subsequently verify .NET 10 with the latest stable ABP 10 at implementation time.

**Spec:** [Shared module design](design.md). This plan refines consumption/status contracts without expanding to HTTP or administration modules. Follow the [shared directory conventions](../README.md).

## Global Constraints

- Implement/verify net9 first, then integrate dev; production uses `$(ProjectFrameworks)`, tests `$(NetCoreFx)`.
- No ASP.NET Core/MVC/HttpContext, ConcurrencyLimiting, EF Core, database, or Redis/concrete lock provider references in production.
- Defaults: IsEnabled=true, LockTimeout=5 seconds, CoordinationScope=Local. Distributed requires matching shared Store/lock scope.
- First successful count starts a fixed window without renewal; each admitted Check consumes once, business failure does not restore it.
- Multiple rules pre-check, then individually consume; no cross-rule transaction/rollback after later competition failure.
- maxCount=0 permanently denies; negative counts/nonpositive durations are invalid; read-only checks do not reserve quota.
- Disable automatic tenant isolation for the counter cache item; rule keys control it explicitly.
- Run dotnet build/test outside the sandbox with `prefix_rule: ["dotnet"]`; no implicit commits, merges, or pushes.

## Review Focus

- Last admitted call succeeds even when its resulting quota is exhausted: Tasks 2/3.
- Physically retained expired items do not block; the window deadline does not renew: Task 2.
- Shared cache/local lock is not distributed atomicity; failures are not misses: Task 2.
- Named-rule count/duration changes retain old state/deadline as specified: Tasks 1/2.
- IsAllowed/GetStatus are snapshots under contention; atomic Check remains required: Task 3.

## File Structure and Public Contracts

Production project: `framework/src/SharpAbp.Abp.OperationRateLimiting`; its `SharpAbp/Abp/OperationRateLimiting` directory is **S** below. Test project: `framework/test/SharpAbp.Abp.OperationRateLimiting.Tests`; its corresponding namespace directory is **T**. Use actual paths during implementation.

Namespace, AssemblyName, and PackageId correspond to SharpAbp.Abp.OperationRateLimiting. The original checker/context/store contracts remain, and the following partition contracts are implemented:

```csharp
public interface IOperationRateLimitingChecker
{
    Task CheckAsync(string policyName,
        OperationRateLimitingContext? context = null,
        CancellationToken cancellationToken = default);
    Task<bool> IsAllowedAsync(string policyName,
        OperationRateLimitingContext? context = null,
        CancellationToken cancellationToken = default);
    Task<OperationRateLimitingStatus> GetStatusAsync(string policyName,
        OperationRateLimitingContext? context = null,
        CancellationToken cancellationToken = default);
    Task ResetAsync(string policyName,
        OperationRateLimitingContext? context = null,
        CancellationToken cancellationToken = default);
}
public interface IOperationRateLimitingStore
{
    Task<OperationRateLimitingStoreResult> IncrementAsync(
        string key, TimeSpan duration, int maxCount,
        CancellationToken cancellationToken = default);
    Task<OperationRateLimitingStoreState> GetAsync(
        string key, CancellationToken cancellationToken = default);
    Task ResetAsync(string key, CancellationToken cancellationToken = default);
}
public interface IOperationRateLimitingRule
{
    Task<OperationRateLimitingRuleStatus> GetStatusAsync(
        OperationRateLimitingRuleContext context, CancellationToken cancellationToken = default);
    Task<OperationRateLimitingRuleResult> ConsumeAsync(
        OperationRateLimitingRuleContext context, CancellationToken cancellationToken = default);
    Task ResetAsync(OperationRateLimitingRuleContext context,
        CancellationToken cancellationToken = default);
}
```

| Type | Semantics |
| --- | --- |
| OperationRateLimitingContext | legacy string? Parameter; named OperationRateLimitingArguments; IDictionary<string, object?> ExtraProperties |
| OperationRateLimitingStoreState | int CurrentCount, DateTime? WindowEndUtc; absent/expired returns 0/null |
| OperationRateLimitingStoreResult | bool IsAccepted and OperationRateLimitingStoreState State; admission of this consumption |
| OperationRateLimitingRuleContext | Definition, business Context, resolved StoreKey; shared model for fixed/custom rules |
| OperationRateLimitingRuleStatus | RuleName, IsAllowed (next admission), MaxCount, CurrentCount, RemainingCount, TimeSpan? RetryAfter |
| OperationRateLimitingRuleResult | bool IsAccepted and OperationRateLimitingRuleStatus Status |
| OperationRateLimitingStatus | IsEnabled, IsAllowed, MaxCount, CurrentCount, RemainingCount, RetryAfter, read-only RuleDetails |
| OperationRateLimitExceededException | BusinessException with policy/rule/count/window metadata; default codes distinguish temporary exhaustion/permanent ban |
| IOperationRateLimitingPolicyProvider.GetAsync(string, CancellationToken) | Task<OperationRateLimitingPolicy>; validated immutable policy; unknown names fail |
| IOperationRateLimitingFormatter.Format(TimeSpan duration) | Localized duration description; Rule/Checker decisions do not depend on formatted text |

IsAccepted and Status.IsAllowed are distinct: for MaxCount=1, first Consume returns IsAccepted=true with RemainingCount=0/IsAllowed=false. Next-call rejection does not reject this successful consumption.

Four parameter convenience extensions accept string parameter and construct Context. `OperationRateLimitingArguments` accepts named values, and checker extensions accept it for composite calls. `OperationRateLimitingPartition` describes `Argument`, built-in identity, and named `Resolver` sources. A rule can contain one or more partitions; a composite key hashes partition names and resolved values. Null literals use the context overload/explicit type to avoid ambiguity. Disabled valid-name calls bypass Provider/Store: Status has IsEnabled=false, IsAllowed=true, MaxCount/RemainingCount=int.MaxValue, CurrentCount=0, RetryAfter=null, empty RuleDetails.

## Task 1: Projects, Policies, Partitions, and Keys

**Files:** Production/test csproj; S/SharpAbpOperationRateLimitingModule.cs, S/SharpAbpOperationRateLimitingOptions.cs, S/OperationRateLimitingPolicy.cs, S/OperationRateLimitingRuleDefinition.cs, S/OperationRateLimitingPolicyBuilder.cs, S/OperationRateLimitingRuleBuilder.cs, S/OperationRateLimitingContext.cs, S/OperationRateLimitingArguments.cs, S/OperationRateLimitingPartition.cs, S/OperationRateLimitingCoordinationScope.cs, S/IOperationRateLimitingPolicyProvider.cs, S/OptionsOperationRateLimitingPolicyProvider.cs, S/OperationRateLimitingPartitionResolver.cs, S/IOperationRateLimitingCompositePartitionResolver.cs, S/OperationRateLimitingKeyBuilder.cs; T/SharpAbpOperationRateLimitingTestModule.cs, T/SharpAbpOperationRateLimitingTestBase.cs, T/OperationRateLimitingPolicyTest.cs, T/OperationRateLimitingPartitionTest.cs; central packages/net9 .sln.

**Interfaces:** Produces Policy/RuleDefinition/Provider/Context/key. Options supply AddPolicy(string, Action<OperationRateLimitingPolicyBuilder>), ConfigurePolicy with the same arguments, RemovePolicy(string); options may be chained.

- [x] Create ordinary ABP/Autofac core tests. Add missing central dependencies such as lock abstractions only; no production Web/database/Redis providers.
- [x] Write policy tests: reject empty names/rules, invalid duration/maxCount, duplicate rule names. Same-name AddPolicy replaces; ConfigurePolicy preserves other rules; ClearRules requires adding a valid rule afterward.
- [x] Builder exposes WithFixedWindow(TimeSpan duration, int maxCount), AddRule(Action<OperationRateLimitingRuleBuilder>), AddRule<TRule>() where TRule:IOperationRateLimitingRule, WithErrorCode(string), ClearRules(). RuleBuilder has WithName/WithFixedWindow/WithMultiTenancy/partition methods. Policy shortcut partition/tenant methods modify the same shortcut fixed-window rule rather than adding another rule.
- [x] Partition methods: PartitionGlobally, PartitionByParameter, PartitionByCurrentUser, PartitionByCurrentTenant, PartitionByEmail, PartitionByPhoneNumber, legacy PartitionBy(string resolverName), and composite PartitionBy(params OperationRateLimitingPartition[]). Default is Global. Named resolver signature remains Func<IServiceProvider, OperationRateLimitingContext, CancellationToken, Task<string>>, with Add/Replace; unknown names fail final validation.
- [x] Named OperationRateLimitingArguments and OperationRateLimitingContext helpers provide explicit parameter-name matching without DTO interfaces, annotations, or runtime property reflection.
- [x] Write partition/key tests: invariant uppercase email, normalized phone, distinct ordinary A/a, missing parameter/anonymous CurrentUser errors; explicit tenancy/host behavior; (ab,c) and (a,bc) key components cannot collide.
- [x] Implement length-prefixed UTF-8/SHA-256 stable keys with ApplicationName (default sharp-abp, nonempty), namespace version, policy/rule/partition/required tenant. Independent apps sharing a backend configure distinct names; same deployment agrees. Unnamed definition changes alter keys, named keys remain stable. Hash APIs support netstandard2.0.
- [x] Confirm failing tests, implement validation/frozen definitions, rerun successfully. Custom definitions hold RuleType and validate DI resolution; fixed-window field rules must not be incorrectly imposed on custom algorithms.

## Task 2: Atomic Store and Coordination Scope

**Files:** S/IOperationRateLimitingStore.cs, S/OperationRateLimitingStoreState.cs, S/OperationRateLimitingStoreResult.cs, S/OperationRateLimitingCacheItem.cs, S/DistributedCacheOperationRateLimitingStore.cs; T/OperationRateLimitingStoreTest.cs, T/Fakes/MutableClock.cs, T/Fakes/ControllableCache.cs; startup validation.

**Interfaces:** Consumes Options/key/ABP IClock/typed cache/IAbpDistributedLock; produces replaceable Store and consumption/state results.

- [x] Write LastPermitIsAcceptedTest: maxCount=1 gives first IsAccepted=true/Count=1, second false/Count remains1; one hundred concurrent calls admit exactly one.
- [x] Write WindowBoundaryTest using controllable UTC time: before deadline retains count, at/after deadline resets; increment/named-rule duration change never extends the existing deadline; physically retained expired entries are absent logically.
- [x] Implement locked read/expiration check/conditional increment/fixed-expiration write with LockTimeout=5 seconds. Explicit hideErrors=false for all cache operations; use UTC regardless of host time zone.
- [x] Get returns valid state or 0/null without writes/window creation. Reset uses the same lock key as Increment. Rule permanently rejects zero quota, and Store must not incorrectly admit maxCount=0.
- [x] Write StoreFaultTest: read/write/lock failures and cancellation propagate rather than masquerading as misses/exhaustion; Reset/Increment races do not corrupt state or create negative counts.
- [x] Write DistributedScopeValidationTest: Distributed+LocalAbpDistributedLock fails startup; Local default works. Custom atomic Stores replace default without forced default locking; default lock validation applies only to default Store.
- [x] Set typed-cache IgnoreMultiTenancy and prove global keys share across tenants. ApplicationName/tenant scope match cache/lock; extra host/provider prefixes must agree throughout one deployment.
- [x] Run all tests successfully and build all three production TFMs.

## Task 3: Fixed-Window Rule, Checker, and Extension Points

**Files:** S/IOperationRateLimitingRule.cs, S/OperationRateLimitingRuleContext.cs, S/OperationRateLimitingRuleStatus.cs, S/OperationRateLimitingRuleResult.cs, S/FixedWindowOperationRateLimitingRule.cs, S/IOperationRateLimitingChecker.cs, S/OperationRateLimitingChecker.cs, S/OperationRateLimitingCheckerExtensions.cs, S/OperationRateLimitingStatus.cs, S/OperationRateLimitExceededException.cs, S/OperationRateLimitingErrorCodes.cs, S/IOperationRateLimitingFormatter.cs, S/OperationRateLimitingFormatter.cs; T/OperationRateLimitingCheckerTest.cs, T/CustomOperationRateLimitingRuleTest.cs; localization.

**Interfaces:** Consumes Store/Provider/resolved keys; produces four Checker methods and Rule/Formatter extension contracts.

- [x] Write CheckVersusStatusTest: first Check with maxCount=1 succeeds, subsequent Status.IsAllowed=false; repeated read checks neither consume nor reserve, and Reset permits another Check.
- [x] FixedWindowRule separates consumption from next-call status: Remaining=max(0,maxCount-count); exhausted RetryAfter=max(0,WindowEnd-now). Zero maxCount is permanent with RetryAfter=null/no window.
- [x] Write MultiRuleCompetitionTest: two pre-checks pass, a simulated competitor exhausts rule2 during consumption; Check rejects while rule1's consumption remains. Failed pre-check consumes nothing anywhere.
- [x] Implement ordered consumption and design Section6.3 aggregation: maximum blocked retry, permanent-ban precedence, no sums across partitions, accurate RuleDetails.
- [x] Write DisabledCheckerTest bypassing Provider/Store/resolvers, following specified disabled states/no-op Check/Reset. Business failure still consumes quota.
- [x] Write ExtensionContractTest for replacement Provider/Store/Formatter and scope-resolved custom Rule; all four Checker methods use appropriate behavior. Async resolver cancellation propagates; unknown policy/type/identity is not exhaustion.
- [x] Add module codes/count/window/rule metadata, replaceable formatting resources in all nine FileSystem locales; do not copy all ExtraProperties into Data. Core IHasHttpStatusCode=429 metadata does not add Web dependencies.
- [x] Run focused tests successfully; infrastructure failures remain distinct from business quota exhaustion.

## Task 4: ABP Attributes, Distributed Acceptance, and Documentation

**Files:** S/OperationRateLimitingAttribute.cs, S/DisableOperationRateLimitingAttribute.cs, S/RateLimitingParameterAttribute.cs, S/IHasOperationRateLimitingParameter.cs, S/OperationRateLimitingInvocationContextResolver.cs, S/OperationRateLimitingInterceptor.cs, S/OperationRateLimitingInterceptorRegistrar.cs; T/OperationRateLimitingInterceptorTest.cs, T/OperationRateLimitingSharedStoreTest.cs; docs/OperationRateLimiting.md; module registration.

**Interfaces:** `IHasOperationRateLimitingParameter.GetPartitionParameter(): string?`; interceptor consumes CheckAsync before business execution, without execution concurrency leases.

- [x] Write actual ABP/Autofac proxy tests for Task/Task<T>, class/method/disable priority, marker-over-DTO priority, ambiguous candidates, consumption on business failure, and unknown policy preventing execution.
- [x] Follow the concurrency plan's proxy boundary and unique CancellationToken rules. Do not treat arbitrary ToString as domain normalization; absent candidates are valid only for partitions not requiring Parameter.
- [x] Confirm failure, implement resolution/registration/CheckAsync→ProceedAsync without MVC filters.
- [x] Test two ABP containers sharing controlled cache/lock in SharedCounterAcrossContainersTest: same ApplicationName/policy/key admits at most maxCount; different apps/isolated tenants are independent. One-container contention does not replace this check.
- [ ] Add opt-in real infrastructure tests in this test project, configured by environment without forcing providers into production. Missing configuration skips with a recorded reason; skipped checks are not acceptance. Use at least two processes against one key and record successful totals, reset behavior, and infrastructure failure results.
- [x] Usage docs cover DependsOn/configuration/four APIs/window anchor/normalization/named rule changes/partial consumption/disable/background work/Store replacement/local-distributed requirements. Combination with the other module is manual only.
- [x] Run focused tests, all-TFM production build, framework Release .sln build, git diff --check. Check production/tests avoid Web and the other limiting module.
- [ ] Follow design Section9 for shared dev integration. Record whether real distributed acceptance completed; otherwise claim only local/simulated protocol checks.

## Completion Criteria

Record net9 core completion, real distributed acceptance, and dev compatibility separately. Update dev to the latest stable ABP at implementation time, retain .NET10/.slnx, and rerun relevant APIs/tests. An ABP version update is not feature acceptance.

After acceptance, update this document and the [feature index](../README.md) status and link implemented usage documentation.
