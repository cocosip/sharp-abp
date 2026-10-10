# Operation rate limiting

`SharpAbp.Abp.OperationRateLimiting` limits how many attempts an operation may make within a fixed window. It works in application services, domain services, and background work. It has no ASP.NET Core, MVC, database, or Redis dependency. The default Store uses ABP's typed cache and distributed-lock abstractions.

For one business operation identified by several fields, see [Single and composite partitions](#single-and-composite-partitions). That section includes direct multi-partition and custom resolver examples for business month + tenant + user + phone.

## Configuration

Reference the package and add `SharpAbpOperationRateLimitingModule` to the host module's `DependsOn` list:

```csharp
using SharpAbp.Abp.OperationRateLimiting;
using Volo.Abp.Modularity;

[DependsOn(typeof(SharpAbpOperationRateLimitingModule))]
public class MyApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<SharpAbpOperationRateLimitingOptions>(options =>
        {
            options.ApplicationName = "my-application";
            options.AddPolicy("SendCode", policy => policy
                .WithFixedWindow(TimeSpan.FromMinutes(1), maxCount: 1)
                .PartitionByPhoneNumber()
                .WithMultiTenancy());
        });
    }
}
```

Defaults are `IsEnabled=true`, `ApplicationName="sharp-abp"`, `LockTimeout=5 seconds`, and `CoordinationScope=Local`. The lock timeout covers counter coordination, not business execution. Application names must be nonempty. Independent applications sharing a backend need different names; every instance of one deployment must agree on the name and cache/lock prefixes.

`AddPolicy` replaces an existing policy with the same name. `ConfigurePolicy` updates an existing policy while retaining its rules. `ClearRules` clears rules and must be followed by adding a valid rule; `RemovePolicy` removes a policy. Repeated custom resolver registration also replaces the previous resolver. Rule names inside a policy must be unique. Empty policies, negative counts, nonpositive fixed-window durations, unknown resolver names, and unresolved custom rule types fail configuration. The default provider captures immutable startup configuration; changing an options object after startup does not change a running deployment.

## Manual APIs

Inject `IOperationRateLimitingChecker`:

```csharp
await checker.CheckAsync("SendCode", "+86 (123) 456", cancellationToken);
await SendCodeAsync(cancellationToken);

bool allowed = await checker.IsAllowedAsync("SendCode", "+86123456", cancellationToken);
OperationRateLimitingStatus status = await checker.GetStatusAsync(
    "SendCode", "+86123456", cancellationToken);
await checker.ResetAsync("SendCode", "+86123456", cancellationToken);
```

`CheckAsync` atomically consumes one operation count per applicable rule. The first accepted count starts the window. Reading, incrementing, and changing a named rule's duration do not renew that window. An item whose deadline has passed is logically absent even if the backend still retains its bytes. Reset uses the same per-counter coordination as increment and clears state; it is not a transaction over a policy's rules.

`IsAllowedAsync` and `GetStatusAsync` are read-only snapshots. They neither consume nor reserve quota, and another caller can consume the final count immediately afterward. Always call `CheckAsync` before the operation. The last permitted call succeeds even when the status afterward reports `IsAllowed=false` and `RemainingCount=0`.

Business failure or cancellation after admission does not return the consumed count. Use these APIs to count attempts, not successful outcomes. `maxCount=0` is a permanent denial with no retry deadline. Unknown policies, missing identities, cancellation, lock timeouts, and cache failures remain distinct errors; they do not become quota-exhaustion errors.

The context overload accepts `OperationRateLimitingContext`, with the legacy `Parameter`, named `OperationRateLimitingArguments`, and an `ExtraProperties` dictionary for application-specific resolver data. These properties are not copied wholesale into public exception data. For a null argument, use the context overload or an explicit cast to avoid ambiguity with the string convenience overloads.

## Partitions and rule identity

Partitions default to Global. Builders offer `PartitionGlobally`, `PartitionByParameter`, `PartitionByCurrentUser`, `PartitionByCurrentTenant`, `PartitionByEmail`, `PartitionByPhoneNumber`, and `PartitionBy("resolver-name")`.

Ordinary parameters preserve case and whitespace. Email parameters are trimmed and converted to invariant uppercase. Phone formatting is removed while ASCII digits and `+` remain. Missing parameters and anonymous current-user identities fail explicitly. CurrentTenant represents a tenant ID or the host marker. `WithMultiTenancy` independently adds the current tenant/host dimension; without it, a Global rule shares its counter across tenants. The counter cache item ignores ABP's automatic tenant normalization, so the explicit rule key controls isolation.

```csharp
Configure<SharpAbpOperationRateLimitingOptions>(options =>
{
    options.AddPartitionResolver("Account", async (serviceProvider, context, token) =>
    {
        var resolver = serviceProvider.GetRequiredService<IAccountKeyResolver>();
        return await resolver.ResolveAsync(context.Parameter, token);
    });
    options.AddPolicy("Export", policy => policy
        .WithFixedWindow(TimeSpan.FromHours(1), 5)
        .PartitionBy("Account"));
});
```

Named resolvers run asynchronously in the caller's DI scope and must return a nonempty key. Built-in partition names are reserved. A custom resolver cannot replace them.

Keys use versioned, length-prefixed UTF-8 components and SHA-256. Application, policy, rule, partition, and explicit tenant components are included without separator ambiguity. Unnamed rule identity includes its settings; changing those settings starts a different counter. A named rule keeps its identity when its count/duration changes: a new maximum applies to the existing count, and a new duration applies to the next window while preserving the current deadline. Rolling instances of one deployment should use compatible configuration.

## Single and composite partitions

A rule can use one partition or several partitions. A partition identifies one dimension of the rule's counter; a composite partition combines all configured dimensions into one counter identity. For example, one SMS per minute for each **business month + tenant + user + phone** uses four dimensions in one rule. The business month is caller-supplied business data and remains independent of the one-minute limiting window.

The common direct configuration is:

```csharp
Configure<SharpAbpOperationRateLimitingOptions>(options =>
{
    options.AddPolicy("SendSms", policy => policy
        .WithFixedWindow(TimeSpan.FromMinutes(1), 1)
        .PartitionBy(
            OperationRateLimitingPartition.Argument("BusinessMonth"),
            OperationRateLimitingPartition.CurrentTenant(),
            OperationRateLimitingPartition.CurrentUser(),
            OperationRateLimitingPartition.PhoneNumber("PhoneNumber")));
});
```

The caller passes named values. DTOs do not need limiting interfaces, attributes, or extra properties:

```csharp
var arguments = OperationRateLimitingArguments.Create(
    ("BusinessMonth", "2026-05"),
    ("PhoneNumber", "158 (0000) 0001"));

await checker.CheckAsync("SendSms", arguments, cancellationToken);
```

`CurrentTenant` and `CurrentUser` are resolved from the current ABP scope and do not require caller arguments. Named matching is used for composite calls instead of positional matching, so adding an automatic dimension does not shift business values. `PhoneNumber` removes formatting; `Email` trims and uses invariant uppercase. A single partition remains valid:

```csharp
policy.WithFixedWindow(TimeSpan.FromMinutes(1), 1)
      .PartitionBy(OperationRateLimitingPartition.PhoneNumber("PhoneNumber"));
```

The resulting counter identity is:

```text
Application + Policy + Rule + Hash(PartitionName, PartitionValue, ...)
```

Changing any component selects another counter. Changing the business month does not change the fixed-window duration.

## Custom partition resolvers

A resolver computes the value of one custom partition. It is not required for ordinary caller fields or built-in ABP identities. Use it when a value needs custom derivation, when several arguments should become one business dimension, or when the value must be obtained through a scoped service such as a database repository, cache, or external client.

Register a named resolver in the same style as ABP Pro:

```csharp
options.AddPartitionResolver("CustomerAccount", async (provider, context, token) =>
{
    var customerId = context.GetArgument<Guid>("CustomerId");
    var accountType = context.GetArgument<string>("AccountType");
    var resolver = provider.GetRequiredService<ICustomerAccountResolver>();

    return await resolver.ResolveAsync(customerId, accountType, token);
});
```

Use it alone or mix it with built-in and argument partitions. `Resolver("CustomerAccount")` uses the resolver name as the partition name; `Custom("AccountIdentity", "CustomerAccount")` can give the partition a separate stable name when the same resolver is reused:

```csharp
options.AddPolicy("AccountOperation", policy => policy
    .WithFixedWindow(TimeSpan.FromHours(1), 10)
    .PartitionBy(
        OperationRateLimitingPartition.Resolver("CustomerAccount"),
        OperationRateLimitingPartition.CurrentTenant()));

var arguments = OperationRateLimitingArguments.Create(
    ("CustomerId", customerId),
    ("AccountType", accountType));

await checker.CheckAsync("AccountOperation", arguments, cancellationToken);
```

The resolver can also return a composite value such as `business month + user + phone`. In that form the rule has one custom partition whose value is produced by the resolver. Alternatively, configure those values as separate direct partitions. Both forms are supported; direct partitions are preferable when each dimension is already available, while a resolver is appropriate for custom business rules or external reads. Resolver reads do not store rate-limit state and do not replace the cache and distributed-lock coordination used by the rule store.

The resolver context exposes all named arguments, the current partition name, the current direct argument value, and the existing `ExtraProperties` dictionary. Existing resolvers that read `context.Parameter` remain supported for legacy single-partition policies.

## Which calls share a counter

Assume calls occur in the same one-minute window and start with unused quota:

| Business month | Tenant | User | Phone | Result |
| --- | --- | --- | --- | --- |
| 2026-05 | A | ZhangSan | 15800000001 | First call accepted. |
| 2026-05 | A | ZhangSan | 15800000001 | Rejected; every partition matches. |
| 2026-05 | A | ZhangSan | 15800000002 | Accepted; the phone partition differs. |
| 2026-05 | A | LiSi | 15800000001 | Accepted; the user partition differs. |
| 2026-05 | B | ZhangSan | 15800000001 | Accepted; the tenant partition differs. |
| 2026-06 | A | ZhangSan | 15800000001 | Accepted; the business partition differs. |

`WithMultiTenancy()` remains a compatibility shortcut that adds the current tenant dimension to the legacy single-partition key. For new composite policies, use `OperationRateLimitingPartition.CurrentTenant()` explicitly. Repeated `PartitionBy` calls replace the complete partition list; they do not silently append dimensions.

Multiple `AddRule` entries create separate counters combined with AND. A user rule and a phone rule enforce a user-wide allowance across all phones and a phone-wide allowance across all users. They do not enforce an independent allowance for each user/phone pair. A policy can also contain several rules that each select a composite resolver, for example one-minute and one-hour limits on the same business identity.

## Multiple rules

```csharp
options.AddPolicy("GenerateReport", policy => policy
    .AddRule(rule => rule.WithName("daily")
        .WithFixedWindow(TimeSpan.FromDays(1), 5).PartitionByCurrentUser())
    .AddRule(rule => rule.WithName("hourly")
        .WithFixedWindow(TimeSpan.FromHours(1), 2).PartitionByCurrentTenant()));
```

Rules combine with AND. The checker reads all rules first and, if allowed, consumes each in configuration order. A failed precheck consumes nothing. There is no transaction across counters: if a competitor exhausts a later rule during consumption, earlier consumption remains. There is no unsafe decrement rollback.

Aggregate remaining count is the minimum, not a sum across windows or partitions. Aggregate maximum/current counts come from the rule with the minimum remaining count; ties retain configuration order. A permanent denial takes precedence; otherwise the retry delay is the maximum among blocked rules. `RuleDetails` preserves each rule's status for interpretation.

## Service attributes

```csharp
[OperationRateLimiting("SendCode")]
public class CodeService : ITransientDependency
{
    public virtual async Task SendAsync(
        [RateLimitingParameter] string phone, CancellationToken cancellationToken)
    {
        await SendCodeAsync(phone, cancellationToken);
    }

    [DisableOperationRateLimiting]
    public virtual Task MaintenanceAsync()
    {
        return Task.CompletedTask;
    }
}
```

The module registers an ABP interceptor through `OnRegistered`. It checks before `ProceedAsync`; it holds no execution lease. Method policy overrides class policy. Disable markers on a method or class take precedence. Method/parameter metadata is cached at registration and reused during calls.

An explicit string parameter marker takes precedence over DTO candidates. Otherwise a unique argument implementing `IHasOperationRateLimitingParameter.GetPartitionParameter()` supplies the parameter. Multiple markers, multiple DTO candidates without a marker, or multiple CancellationToken parameters are configuration errors. Arbitrary objects are never normalized through `ToString`. No candidate is valid for a Global partition but fails when the selected partition requires a parameter.

Declarative interception supports DI-created, proxy-eligible `Task`/`Task<T>` methods. Class-proxy methods must be virtual. Synchronous and ValueTask methods use the manual APIs. Direct construction is outside interception. Self-calls have no bypass guarantee: the tested ABP class proxy re-enters interception for a virtual self-call. Avoid applying limits recursively and use a clear external service boundary. Deferred streams or subsequent work are not covered by returning a Task; protect them explicitly with the manual APIs.

## Replacements, localization, and disable

`IOperationRateLimitingStore`, `IOperationRateLimitingPolicyProvider`, `IOperationRateLimitingRule`, and `IOperationRateLimitingFormatter` are replacement contracts. ABP `Dependency(ReplaceServices=true)` and `ExposeServices` can replace default services:

```csharp
[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IOperationRateLimitingStore))]
public class MyAtomicStore : IOperationRateLimitingStore, ISingletonDependency
{
    // Implement IncrementAsync, GetAsync, ResetAsync.
}
```

Store implementations own atomic conditional admission and coordinated reset. A Store result's `IsAccepted` describes this consumption; the resulting rule status's `IsAllowed` describes the next call. An uncoordinated Get/Set implementation is insufficient. Custom rules provide separate status, consume, and reset methods and are resolved in the caller's scope. Register the concrete rule type with DI, then use `AddRule<MyRule>()` or `rule.WithRule<MyRule>()`. Fixed-window duration/count validation is not imposed on custom algorithms.

`OperationRateLimitExceededException` is an ABP BusinessException with core HTTP 429 metadata, policy/rule/count/deadline/retry information, and separate `OperationRateLimiting:LimitExceeded`/`OperationRateLimiting:PermanentlyDenied` codes. No Web package or response-header writer is included. Policies can select `WithErrorCode("MyApp:LimitExceeded")` and use the host's localization mapping. The default resource and mapped error-code namespace are both `OperationRateLimiting`, with en, zh-Hans, zh-Hant, vi, tr, sl, pt-BR, pl-PL, and cs resources. The replaceable formatter describes durations; admission decisions never depend on formatted text.

When `IsEnabled=false`, valid-name Check/Reset calls do nothing and do not access the provider, resolver, or Store. IsAllowed returns true. Status has `IsEnabled=false`, `IsAllowed=true`, `MaxCount=RemainingCount=int.MaxValue`, `CurrentCount=0`, no retry delay, and no rule details. Empty names remain invalid.

## Coordination and acceptance

The default ABP memory cache and LocalAbpDistributedLock support one application container in one process. Sharing Redis cache with a local lock does not establish distributed atomicity. For cross-process counting, select `CoordinationScope=Distributed` and configure a cache plus a lock provider spanning exactly the same deployment instances. Startup rejects LocalAbpDistributedLock for the default Store in Distributed mode. This check cannot prove arbitrary providers share data/coordination; deployment acceptance is still required. Custom atomic Stores replace the default without a forced default lock requirement.

Local tests cover actual ABP/Autofac proxies, cache/lock faults, clock boundaries, and two ABP containers sharing a controlled cache/lock. These are local/simulated protocol checks, not real distributed acceptance.

The test project includes an opt-in two-process Redis harness using test-only StackExchange.Redis cache and Medallion renewable locks. Set `SHARP_ABP_OPERATION_REDIS` in the environment to a dedicated Redis configuration, then run:

```powershell
dotnet test framework/test/SharpAbp.Abp.OperationRateLimiting.Tests/SharpAbp.Abp.OperationRateLimiting.Tests.csproj -c Release --filter FullyQualifiedName~OperationRateLimitingRedisAcceptanceTest --logger "console;verbosity=detailed"
```

The harness starts two processes, synchronizes their start, verifies that their combined accepted count is exactly seven, resets through the same Store, and verifies seven new admissions. It uses a random application namespace, clears only its own counter/gate keys, and passes connection configuration through inherited environment variables. It records admission totals and infrastructure-failure types without printing configuration/credentials. With missing configuration the real-backend test is explicitly skipped; this is not acceptance. A separate local worker test verifies that missing configuration exits as infrastructure failure, not admission or exhaustion.

Operation limiting and concurrency limiting are independent packages. For explicit manual composition, acquire a concurrency lease first, consume operation quota second, execute the operation, and dispose the lease in all outcomes. Counts are not returned. Automatic ordering of dual attributes is not supported.
