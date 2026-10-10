# Concurrency Limiting and Operation Rate Limiting Design

Date: 2026-10-10

Status: In Progress; net9 core implemented and isolated .NET 10 compatibility verified. Real distributed acceptance and dev integration remain pending. See [validation results](validation.md).

This design covers one module group in the shared [feature design directory](../README.md). Future modules have their own feature directories and designs.

## 1. Goals and Scope

Add two independently usable infrastructure modules under `framework/src`:

1. **ConcurrencyLimiting** limits operations executing at the same time, using Microsoft's `System.Threading.RateLimiting.ConcurrencyLimiter`.
2. **OperationRateLimiting** limits operation counts within a time window, following ABP Pro's documented `AbpOperationRateLimitingOptions` capabilities.

Both serve application services, domain services, and background execution. They do not reference each other, ASP.NET Core, MVC, HttpContext, or business modules. Each provides manual APIs and ABP service interception for its own limiting behavior.

The scope includes only two core modules and their core tests. HTTP pipeline throttling remains a host concern using ASP.NET Core's existing middleware. No ASP.NET Core extension packages or Web test projects are planned.

Implement and verify on **`net9` first, then merge `net9` into `dev` and verify .NET 10**. The workspace is currently on net9. Core implementation is now authorized. Commits and branch integration are separate delivery steps; database-backed storage design and implementation remain out of scope.

## 2. ABP Pro Research

### 2.1 Operation Counts

ABP's official module is **Operation Rate Limiting Module (Pro)**. Examples include three verification messages per phone number per hour, two reports per user per day, and login attempt limits per IP address.

`WithFixedWindow(duration, maxCount)` restricts counts within a time window. A successful `CheckAsync` consumes quota; completing the operation does not return it.

The public documentation does not describe a built-in `WithConcurrency`, execution lease, or waiting queue. Custom rule extension points are not evidence of built-in execution concurrency limiting.

### 2.2 Documented Capabilities

| Capability | Documented ABP Pro Behavior |
| --- | --- |
| Named policies | AddPolicy, replacement by name, partial updates through ConfigurePolicy |
| Rules | Fixed windows; multiple rules combined with AND |
| Partitions | Parameter, current user, current tenant, client IP, email, phone number, named custom resolvers |
| Multiple tenants | Explicit WithMultiTenancy; no automatic tenant dimension for every rule |
| Manual APIs | CheckAsync, IsAllowedAsync, GetStatusAsync, ResetAsync |
| Declarative usage | OperationRateLimiting attribute on classes or methods |
| Parameter extraction | RateLimitingParameter first, then IHasOperationRateLimitingParameter on a DTO |
| Service invocation | ABP dynamic proxies, subject to interception requirements |
| MVC actions | An action filter in a separate ASP.NET Core package |
| Rejection | Business exception, HTTP 429, remaining counts and retry information |
| Disable switch | Global IsEnabled |
| Ban policies | maxCount=0 permanently denies without a retry window |
| Extension points | Replaceable Store, Rule, PolicyProvider, Formatter |

IsAllowedAsync and GetStatusAsync neither consume nor reserve quota. Callers still invoke CheckAsync before execution; an earlier read is not an atomic admission guarantee.

Multiple rules are not a transaction across counters. The documentation describes checking all rules first and incrementing individually. A later rule can reject because of concurrent consumption, while earlier consumed quota remains consumed.

### 2.3 Foundation and Evidence Limits

Official documentation specifies ABP IDistributedCache as the default Store foundation, IAbpDistributedLock for counter coordination, a five-second LockTimeout, and IOperationRateLimitingRule for custom algorithms. Operation limiting works at a different layer from ASP.NET Core's HTTP middleware.

The documented default mechanism is cache-based counting with lock coordination. Commercial source code was not inspected, so this research does not prove that all internal components avoid Microsoft's limiting APIs. There is no public evidence that the module is a ConcurrencyLimiter wrapper.

Microsoft's ConcurrencyLimiter belongs to the standalone System.Threading.RateLimiting library. It works without ASP.NET Core; the Web middleware adds HTTP integration on top of the same library.

## 3. Repository and Version Constraints

The snapshot below was checked against local dev files and the local net9 reference before switching to net9. Remote references were not refreshed. Recheck versions when implementation begins.

| Setting | net9 | dev |
| --- | --- | --- |
| NetCoreFx | net9.0 | net10.0 |
| ProjectFrameworks | netstandard2.0;netstandard2.1;$(NetCoreFx) | Same expression |
| ABP version range | [9.3.7,10.0.0) | Currently [10.6.1,); update to the latest stable release at implementation time before integration |
| Framework solution | framework/SharpAbp.sln | framework/SharpAbp.slnx |
| Package versions | Central management in Directory.Packages.props | Same |

Publish new capabilities as separate infrastructure packages rather than adding them to SharpAbp.Abp.Core. The existing SecurityKeyController uses EnableRateLimiting for HTTP throttling, separate from this design.

The two new production projects explicitly target `netstandard2.0;netstandard2.1;$(NetCoreFx)`; tests use `$(NetCoreFx)`. The shared `ProjectFrameworks` property currently expands before `NetCoreFx` is assigned, so the explicit expression keeps all three targets confined to these packages without changing other modules. ABP 9.3.7 public APIs form the compatibility baseline. Its IAbpDistributedLock, IHasHttpStatusCode, and interceptor registration were checked. Microsoft's RateLimiting source includes netstandard2.0. Restore and build every production target with selected versions during implementation.

## 4. Packages and Dependencies

| Proposed Package | Responsibilities | Main Dependencies |
| --- | --- | --- |
| SharpAbp.Abp.ConcurrencyLimiting | Policies, partitions, leases, manual APIs, service interception | ABP foundation/identity/tenants; System.Threading.RateLimiting |
| SharpAbp.Abp.OperationRateLimiting | Policies, fixed-window counting, partitions, Checker, Store, service interception | ABP foundation/identity/tenants/caching/lock abstractions/clock |

Add only SharpAbp.Abp.ConcurrencyLimiting.Tests and SharpAbp.Abp.OperationRateLimiting.Tests. Ordinary ABP TestBase/Autofac handles policy, manual API, proxy, and Store tests; do not reference ASP.NET Core TestBase or start Web hosts.

No entities, migrations, administration UI, or business dependencies are required. ConcurrencyLimiting does not depend on caches or distributed locks. OperationRateLimiting references abstractions without forcing Redis, a database, or a concrete lock provider. No additional common limiting framework package is planned.

Module classes are proposed as SharpAbpConcurrencyLimitingModule and SharpAbpOperationRateLimitingModule.

## 5. ConcurrencyLimiting

### 5.1 Semantics

The same policy and partition share a native ConcurrencyLimiter. Each operation acquires one permit and holds it until execution actually ends. Completion, failure, and cancellation all return it through Dispose.

Initially there are no weighted operations or multiple-permit requests. Scope is **one application container in one process**. Three processes with PermitLimit=10 may execute thirty operations in total; this is not cross-process limiting.

### 5.2 Policies and Defaults

Proposed SharpAbpConcurrencyLimitingOptions:

- IsEnabled=true; operations without an applied policy are unrestricted.
- AddPolicy defines/replaces policies; ConfigurePolicy modifies existing ones.
- Each policy explicitly sets PermitLimit>0; no implicit global capacity.
- QueueLimit=0 rejects immediately by default. It maps to the native setting; one permit per operation makes it also the waiting-operation capacity.
- QueueProcessingOrder=OldestFirst and a configurable thirty-second queue timeout.
- Global, Parameter, CurrentUser, CurrentTenant, and named custom partitions. Callers supply IP or other host context through parameters/resolvers; tenant isolation is explicit.
- CurrentUser requires authentication. Missing parameters or empty custom keys produce context/configuration errors rather than allowing execution.

Configuration example:

```csharp
Configure<SharpAbpConcurrencyLimitingOptions>(options =>
{
    options.AddPolicy("ReportGeneration", policy => policy
        .WithConcurrency(permitLimit: 4, queueLimit: 20)
        .WithQueueTimeout(TimeSpan.FromSeconds(30))
        .PartitionByCurrentTenant());
});
```

### 5.3 Manual APIs

| IConcurrencyLimitingManager Method | Behavior |
| --- | --- |
| TryAcquire | Immediate attempt, lease with IsAcquired, no queuing |
| AcquireAsync | Policy-controlled queuing; lease on success; limiting exception on rejection/timeout |
| GetStatistics | Snapshot of available permits/queued operations without acquiring capacity |
| ExecuteAsync | Wraps Task/Task<T> delegates and manages the lease |

Callers dispose leases. Disabled mode returns a successful no-op lease. Statistics do not reserve permits.

```csharp
using var lease = await manager.AcquireAsync(
    "ReportGeneration", context, cancellationToken);

await GenerateReportAsync(cancellationToken);
```

Caller cancellation retains OperationCanceledException. Internal queue timeout uses a distinct limiting reason. Linked tokens must handle acquisition/cancellation races and release acquired leases not delivered to the caller.

### 5.4 Lifetime and Partitions

The registry is singleton so transient services/scopes share limits. Native PartitionedRateLimiter can provide partition reuse and idle cleanup.

Resolve user/tenant/custom keys in the current DI scope before passing immutable policy identity/key to the registry. Do not capture HttpContext, scoped services, or business instances in singleton factories.

Concurrency resolvers are synchronous to retain synchronous TryAcquire/GetStatistics. Callers asynchronously resolve keys needing I/O beforehand and pass Parameter. A transient/scoped Manager shares the registry.

Policies become immutable startup snapshots; no live reconfiguration. Verify high-cardinality idle cleanup without evicting executing/waiting partitions. No ResetAsync: a new limiter coexisting with outstanding old leases could exceed capacity.

### 5.5 Declarative APIs

- ConcurrencyLimiting("PolicyName") applies to classes/methods; method overrides class.
- DisableConcurrencyLimiting skips class policy.
- ConcurrencyLimitingParameter or IHasConcurrencyLimitingParameter supplies the key; ambiguous candidates are errors.
- Acquire a lease, await ProceedAsync, release in finally.
- Initially require interceptable Task/Task<T> under ABP 9.3.7; class proxies require virtual methods. Synchronous callers use manual TryAcquire; do not block for asynchronous queues. Declarative ValueTask is excluded initially.
- No implicit reentrant allowance. Choose a boundary that does not wait for its own permit.

Method completion does not cover subsequent lazy stream enumeration, IAsyncEnumerable, or response transmission. Protect deferred work explicitly with manual APIs.

## 6. OperationRateLimiting

### 6.1 Policies and Public APIs

SharpAbpOperationRateLimitingOptions follow Pro's usage style. Defaults: IsEnabled=true, LockTimeout=5 seconds, CoordinationScope=Local. LockTimeout covers counter coordination, not business execution.

ApplicationName defaults to sharp-abp, forming the common counter/lock namespace. Independent applications sharing a backend use different names; one deployment uses the same name. Names must be nonempty for every Store scope.

```csharp
Configure<SharpAbpOperationRateLimitingOptions>(options =>
{
    options.AddPolicy("SendSmsCode", policy => policy
        .WithFixedWindow(TimeSpan.FromMinutes(1), maxCount: 1)
        .PartitionByParameter()
        .WithMultiTenancy());
});

await checker.CheckAsync("SendSmsCode", phoneNumber, cancellationToken);
```

IOperationRateLimitingChecker provides CheckAsync, IsAllowedAsync, GetStatusAsync, ResetAsync, and parameter convenience methods. Context contains the legacy Parameter, named OperationRateLimitingArguments, and ExtraProperties. Status contains IsAllowed, MaxCount, CurrentCount, RemainingCount, nullable RetryAfter, and rule details.

Consumption IsAccepted means this call obtained quota; status IsAllowed describes the next call. The last permitted call succeeds despite a resulting RemainingCount=0.

Support single or composite Parameter, Argument, CurrentUser, CurrentTenant, Email, PhoneNumber, and named custom resolver partitions. IP is explicit host context, not a core HTTP-dependent PartitionByClientIp. Email uses invariant uppercase; phone formatting is removed while digits/+ remain; ordinary parameters are not normalized. Composite calls match named arguments rather than positional arguments.

Named resolvers may be asynchronous in the current scope. A resolver computes one custom partition value and can combine named arguments or use injected repositories, caches, or external clients. Direct partitions remain preferable for values already available in the invocation. Missing inputs and unknown policies/resolvers are errors rather than unrestricted policies.

### 6.2 Fixed Windows and Store

The first successful count starts the partition's window, ending at that time plus duration. No wall-clock alignment or renewal on access. This is a design choice; Pro documentation does not establish its exact anchor.

Cache state contains CurrentCount and a UTC window end with fixed absolute expiration. Expired state counts as absent even before physical backend removal.

IOperationRateLimitingStore provides:

1. IncrementAsync(key, duration, maxCount, cancellationToken): atomic conditional increment; rejection does not increase count.
2. GetAsync: read without consumption; absent/expired state means zero usage/full quota.
3. ResetAsync: clear state with coordination matching increment.

The default uses typed IDistributedCache and IAbpDistributedLock. Locks cover counters, not business execution; no new distributed locking product is implemented.

**Store owns atomicity.** Default Store acquires/releases its lock; Checker does not. Custom Stores can coordinate conditional consumption/reset with locks or atomic cache operations such as Redis scripts. Uncoordinated Get/Set is insufficient. Database-backed stores are not designed or implemented in this scope.

ABP's default memory cache/local lock supports one process. Distributed counting requires a shared cache and a lock spanning the same instances. Shared Redis with a local lock is not cross-instance correctness.

CoordinationScope defaults to Local. In Distributed mode, default Store startup validation rejects LocalAbpDistributedLock and the host declares shared cache/lock scope. Arbitrary providers' sharing cannot be proven from ABP abstractions; deployment configuration and real acceptance remain required.

Explicitly disable cache error hiding. Lock timeouts/cache failures/configuration errors propagate as such; do not turn them into quota-exhaustion 429 or silently allow execution.

### 6.3 Multiple Rules and Status

Policies have one or more AND rules with potentially distinct partitions/durations. First read every rule, then consume individually in configuration order. No cross-rule transaction: a later race rejection leaves earlier consumption in place, without unsafe unconditional decrement rollback.

Business failure does not restore quota: the Checker counts attempts/invocations. Counting successful outcomes only is excluded initially.

Do not sum counts from different partitions/windows. Aggregate IsAllowed requires all rules; RemainingCount is the minimum. Any ban means permanent denial; otherwise blocked-rule RetryAfter is the maximum waiting time. Aggregate MaxCount/CurrentCount use the minimum-remaining rule, ties resolved by configuration order. RuleDetails supply accurate interpretation.

maxCount=0 permanently denies without a window/RetryAfter; negative count or nonpositive duration is invalid.

### 6.4 Keys and Tenants

Logical keys include application namespace, version, policy, rule, every partition name/value pair, and explicitly enabled tenant ID/host marker. Cache and lock use matching scopes.

Each rule supports one or more partitions. `PartitionBy(params OperationRateLimitingPartition[])` replaces the complete partition list; it does not append dimensions. A direct composite can combine `Argument("BusinessMonth")`, `CurrentTenant()`, `CurrentUser()`, and `PhoneNumber("PhoneNumber")`. A named resolver remains a first-class single partition provider: it may derive a value from named arguments, combine several values into one custom identity, or obtain data through dependency injection. Resolvers can be mixed with direct partitions.

Business dates, billing periods, order identifiers, and other caller-provided values can be named arguments in `OperationRateLimitingArguments`. For example, a business month, tenant, user, and phone can identify a counter with a one-minute limiting window. The business month is independent of the execution clock and does not change the fixed-window algorithm. Choosing calendar-window algorithms is a separate concern.

Composite identity and multiple AND rules have different meanings. A user/phone composite allows each user an independent allowance for each phone. Separate user and phone rules enforce a user-wide allowance across all phones and a phone-wide allowance across all users. See the [operation usage documentation](../../OperationRateLimiting.md) for the composite SMS example.

Rule names are unique. Unnamed identity derives from duration/count/partition/tenant settings, so changes create a new key. Named rules keep their key: maximum changes affect current count immediately; duration changes apply next window without renewing the current deadline.

Stable encoding and hashing prevent separator collisions/oversized partition keys, a deliberate difference from Pro's plaintext partition cache keys. Disable typed-cache automatic tenant normalization for the counter item; the rule key alone controls tenant isolation.

### 6.5 Extension and Disable Behavior

- AddPolicy replaces by name; ConfigurePolicy modifies; ClearRules clears.
- Replaceable Store/PolicyProvider/Rule/Formatter without required database/UI.
- Custom Rules separate read status, consumption, reset; a read is not atomic consumption.
- Default Provider reads startup configuration; dynamic external providers own update behavior, with no promise of live options mutation.
- Disabled valid-name Check/Reset do not access Provider/resolvers/Store; IsAllowed=true. Status has IsEnabled=false, IsAllowed=true, MaxCount/RemainingCount=int.MaxValue, CurrentCount=0, RetryAfter=null, empty RuleDetails. Empty names remain invalid.
- Per-policy error codes/localization; ExtraProperties are available to resolvers without wholesale copying into public exception Data.

Sliding windows/token buckets are extension possibilities, not initial built-ins. A single-process Microsoft algorithm wrapper does not establish distributed quota guarantees.

## 7. ABP Service Interception and Non-Web Usage

Each module owns attributes, parameter markers, disable markers, and its service interceptor without Web dependencies. OperationRateLimiting parameter priority is explicit marker, unique IHasOperationRateLimitingParameter DTO, then no parameter. Method overrides class; disable wins.

All service/background contexts can use manual APIs. DI proxy-eligible Task/Task<T> methods can use attributes; class methods are virtual. Direct new/self-invocation/nonproxyable methods are outside automatic interception guarantees and use manual APIs.

No MVC filter, middleware, request IP reader, or Retry-After header writer is planned. A Web host calling a proxied service is limited at the business boundary.

Independent BusinessException types/error codes carry concurrency policy/PermitLimit/QueueLimit/reason or operation policy/rule/window/counts/nullable RetryAfter. Background callers need no HTTP types. ABP core IHasHttpStatusCode may provide 429 metadata without referencing ASP.NET Core. Concurrency has no predictable retry delay; existing host pipelines own headers/content negotiation.

## 8. Combining the Modules

Example: shared five-report daily quota per user, with four concurrent reports per process.

Manual composition explicitly acquires the concurrency lease, consumes operation quota, executes business work, and always releases the lease. Quota is not returned. Concurrency rejection/queue cancellation does not consume quota; later quota rejection immediately returns the acquired permit.

Automatic dual-attribute ordering is excluded initially; no combination module or mutual dependencies. Use explicit manual composition:

```csharp
using var lease = await concurrencyManager.AcquireAsync(
    "ReportGeneration", concurrencyContext, cancellationToken);
await operationChecker.CheckAsync(
    "DailyReportQuota", operationContext, cancellationToken);
await GenerateReportAsync(cancellationToken);
```

This works in background/ordinary services; either module also works independently.

## 9. net9 Implementation and dev Integration

1. Recheck net9 state/instructions. Implement projects/tests/usage docs there, retaining .NET 9/ABP 9.3.7.
2. Add only necessary central dependencies. Select available stable 9.0.x/10.0.x RateLimiting per branch and verify netstandard assets; no broad net9 upgrade or ABP range expansion.
3. Register two production/two test projects in framework/SharpAbp.sln on net9.
4. Build all production TFMs, run both focused tests, build framework Release.
5. Update dev ABP 10 packages together to the latest stable release at implementation time, retaining .NET 10. The [10.6.1,) range does not ensure latest-version selection: check releases, explicitly update central baseline, verify resolved versions/compatibility; no previews.
6. Merge net9 into dev, retain dev framework/dependencies/.slnx, register new projects in framework/SharpAbp.slnx; do not replace it with net9 .sln.
7. Repeat validation on dev with equivalent APIs/behavior. Resolve compatibility changes before integration is complete.

Verification commands:

```powershell
# net9
dotnet build framework/SharpAbp.sln -c Release
# dev
dotnet build framework/SharpAbp.slnx -c Release
# Run both new test projects on each branch
dotnet test framework/test/SharpAbp.Abp.ConcurrencyLimiting.Tests/SharpAbp.Abp.ConcurrencyLimiting.Tests.csproj -c Release
dotnet test framework/test/SharpAbp.Abp.OperationRateLimiting.Tests/SharpAbp.Abp.OperationRateLimiting.Tests.csproj -c Release
git diff --check
```

In Codex App, run dotnet build/test outside the sandbox with `prefix_rule: ["dotnet"]`, following repository instructions.

## 10. Acceptance Criteria

| Area | Required Checks |
| --- | --- |
| Execution count | N permits admit at most N operations; failures/cancellation restore capacity |
| Queues | Capacity, FIFO, timeout, cancellation, acquisition races, shutdown waiters |
| Reuse | Shared same policy/partition across scopes; independent distinct partitions |
| Windows | Expiration boundaries, no renewal, bans, read-only checks, reset, business failure consumption |
| Counting | At most maxCount accepted calls under contention; no uncoordinated read/write |
| Multiple rules | AND, partition independence, pre-checks, retained earlier consumption after a later race |
| Tenants | Global sharing, explicit isolation, host side, no accidental cache tenant isolation |
| Configuration | Unknown names, duplicate rule names, invalid inputs/resolvers/identity, policy replacement |
| ABP usage | Class/method/disable priority, parameters, DI/virtual requirements, background calls |
| Dependencies | No ASP.NET Core/MVC/Web TestBase or references between core modules |
| Composition | Concurrency rejection/cancellation consumes no quota; quota rejection releases permit |
| Failures | Infrastructure failures are neither quota exhaustion nor silent allowance |
| Versions | Multi-TFM builds/tests on net9/ABP 9 and dev/ABP 10 |

Memory/fake Store tests do not prove cross-instance correctness. Test two service containers sharing a controlled Store/lock, then run real shared-cache/distributed-lock integration acceptance. Record unavailable infrastructure as an unverified boundary, not completed distributed acceptance.

## 11. Implementation Sequence

1. Verify ABP 9 service proxies and multi-target dependencies.
2. Complete concurrency policies, native reuse, leases, queues, manual APIs, lifetime tests.
3. Complete operation policies, partitions, fixed windows, Checker, multi-rule/failure tests.
4. Complete each service interceptor, exceptions, localization, non-Web manual composition tests.
5. Finish net9 documentation/validation, integrate dev while preserving branch metadata, verify .NET 10.

Development was authorized on 2026-10-10. Exclusions remain database-backed storage, Web extensions/tests, cross-process ConcurrencyLimiter, live permit changes, administration UI, and all-algorithm coverage.

## 12. Implementation Plans and Documentation

Read this design together with the relevant plan, which refines contracts/files/checks without expanding scope:

- [ConcurrencyLimiting implementation plan](concurrency-limiting-implementation-plan.md)
- [OperationRateLimiting implementation plan](operation-rate-limiting-implementation-plan.md)

The plans can be implemented independently. After both pass net9 acceptance, perform Section 9 integration/ABP update. Record task progress, actual versions, commands, and results; unexecuted checks remain unchecked.

Implementation produces docs/ConcurrencyLimiting.md and docs/OperationRateLimiting.md for actual supported APIs. Retain this design under docs/designs, link the completed usage docs, and update the document/index status after acceptance. Proposals do not represent released capabilities.

## 13. References

- [ABP Pro Operation Rate Limiting](https://abp.io/docs/latest/modules/operation-rate-limiting)
- [ABP 10.6.1 documentation](https://github.com/abpframework/abp/blob/10.6.1/docs/en/modules/operation-rate-limiting.md)
- [ABP operation limiting article](https://github.com/abpframework/abp/blob/dev/docs/en/Community-Articles/2026-03-10-Operation-Rate-Limiting-in-ABP-Framework/POST.md)
- [Microsoft ConcurrencyLimiter source](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Threading.RateLimiting/src/System/Threading/RateLimiting/ConcurrencyLimiter.cs)
- [RateLimiting target frameworks](https://github.com/dotnet/runtime/blob/v10.0.0/src/libraries/System.Threading.RateLimiting/src/System.Threading.RateLimiting.csproj)
- [ASP.NET Core Rate Limiting](https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit)
- [ABP 9.3.7 IAbpDistributedLock](https://github.com/abpframework/abp/blob/9.3.7/framework/src/Volo.Abp.DistributedLocking.Abstractions/Volo/Abp/DistributedLocking/IAbpDistributedLock.cs)
- [ABP 9.3.7 default local lock](https://github.com/abpframework/abp/blob/9.3.7/framework/src/Volo.Abp.DistributedLocking.Abstractions/Volo/Abp/DistributedLocking/LocalAbpDistributedLock.cs)
- [ABP 9.3.7 HTTP exception mapping](https://github.com/abpframework/abp/blob/9.3.7/framework/src/Volo.Abp.AspNetCore/Volo/Abp/AspNetCore/ExceptionHandling/DefaultHttpExceptionStatusCodeFinder.cs)
