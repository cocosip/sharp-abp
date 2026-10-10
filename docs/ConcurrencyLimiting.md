# Concurrency limiting

`SharpAbp.Abp.ConcurrencyLimiting` provides process-local execution permits for application services, domain services, and background work. It uses Microsoft's `System.Threading.RateLimiting` and does not require ASP.NET Core, a cache, a database, or a distributed lock.

Add `SharpAbpConcurrencyLimitingModule` to the module dependency graph and configure immutable startup policies:

```csharp
[DependsOn(typeof(SharpAbpConcurrencyLimitingModule))]
public class ReportsModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<SharpAbpConcurrencyLimitingOptions>(options =>
        {
            options.AddPolicy("Reports", policy => policy
                .WithConcurrency(4, queueLimit: 20)
                .WithQueueTimeout(TimeSpan.FromSeconds(30))
                .PartitionByCurrentTenant());
        });
    }
}
```

`AddPolicy` and `AddPartitionKeyResolver` replace same-name entries; the last definition wins. `ConfigurePolicy` changes an existing policy while preserving fields that the callback does not touch. It may be called repeatedly. Unknown policy names fail, and retained builders cannot mutate built policies. Named custom resolvers are configured as follows:

```csharp
Configure<SharpAbpConcurrencyLimitingOptions>(options =>
{
    options.AddPartitionKeyResolver("WorkerGroup", (services, context) =>
        services.GetRequiredService<IWorkerContext>().GroupName);
    options.AddPolicy("WorkerReports", policy => policy
        .WithConcurrency(2)
        .PartitionBy("WorkerGroup"));
});
```

Policies default to `QueueLimit = 0`, oldest-first processing, and a 30-second queue timeout. `PartitionGlobally`, `PartitionByParameter`, `PartitionByCurrentUser`, `PartitionByCurrentTenant`, and named synchronous resolvers are available. Parameters are case-sensitive and are not normalized. Missing users, empty parameters, unknown policies, and unknown resolvers are configuration/context errors. `PartitionByCurrentTenant` already uses the current tenant or stable host marker as its partition value. `WithMultiTenancy` adds that tenant/host dimension separately, which is useful when another partition type also needs tenant isolation.

Manual APIs are available through `IConcurrencyLimitingManager`:

```csharp
using var lease = await manager.AcquireAsync("Reports", context, cancellationToken);
await GenerateReportAsync(cancellationToken);

using var immediate = manager.TryAcquire("Reports", context);
if (!immediate.IsAcquired)
{
    // TryAcquire never queues.
}

await manager.ExecuteAsync("Reports", GenerateReportAsync, context, cancellationToken);
```

Every acquired lease remains held until disposal. `ExecuteAsync` disposes in a `finally` path for success, failure, and cancellation. Repeated lease disposal is safe. Caller cancellation remains `OperationCanceledException`; queue timeout and capacity/queue-full rejection use `ConcurrencyLimitExceededException`. Disabled mode returns successful no-op leases and does not resolve policies or create native limiters.

The exception includes the policy name, permit limit, queue limit, and `Capacity`, `QueueFull`, or `QueueTimeout` reason. It has no predictable retry delay. `WithErrorCode` selects an application error code; the application owns localization for custom namespaces. The default `ConcurrencyLimiting` resource is registered in ABP's virtual file system and exception-code mapping with the same nine locales as FileSystem.

Background jobs can inject the Manager and await the full work inside `ExecuteAsync`; the job does not require a Web host. A statistics snapshot reports available permits and queued operations but does not reserve admission. Disabled statistics are `IsEnabled=false`, `AvailablePermits=int.MaxValue`, and `QueuedOperations=0`. Empty names and null operation delegates remain invalid while disabled.

Declarative interception supports virtual `Task` and `Task<T>` service methods:

```csharp
[ConcurrencyLimiting("Reports")]
public class ReportService : ITransientDependency
{
    public virtual async Task GenerateAsync(CancellationToken cancellationToken)
    {
        await GenerateReportAsync(cancellationToken);
    }
}
```

`ConcurrencyLimiting("Reports")` applies at class, interface, or method level, with a method policy overriding its class or interface policy. `DisableConcurrencyLimiting` wins. A marked string `ConcurrencyLimitingParameter` has priority over a unique `IHasConcurrencyLimitingParameter` DTO. Multiple markers, multiple DTOs, multiple cancellation tokens, synchronous methods, and `ValueTask` methods fail explicitly at the first proxied call. Interception is registered through ABP `OnRegistered`; metadata is cached when the service is registered, and business invocation uses ABP `ProceedAsync`. Attributes on an implementation, its interface, or its methods are supported. Direct construction and non-virtual methods bypass interception. Self-invocation has no interception guarantee; some virtual class proxies may re-enter the interceptor. Protect a suitable boundary with the manual APIs when automatic interception does not apply. Nested acquisitions have no reentrant allowance and may wait for their own permit.

The registry and native partitioned limiters are singleton and shared by scopes in one application container. Resolver delegates run in the current scope and are not retained by the registry. The module provides process-local limits; cross-process coordination and live policy changes are outside this package.

Native idle cleanup removes unused partitions while preserving active and queued partitions. Completing `Task`/`Task<T>` work returns its permit; subsequent lazy stream enumeration and response transmission are not part of that lifetime. Explicitly hold a manual lease around deferred work.

A QIDO-style immediate busy gate can use a global zero-queue policy configured from application options at startup:

```csharp
// maxConcurrentRequests is the startup value from IOptions<QidoOptions>.
options.AddPolicy("Qido", policy => policy
    .WithConcurrency(maxConcurrentRequests, queueLimit: 0)
    .PartitionGlobally());

public virtual ConcurrencyLimitingLease Enter()
{
    var lease = Manager.TryAcquire("Qido");
    if (lease.IsAcquired)
    {
        return lease;
    }

    lease.Dispose();
    throw new BusinessException("DicomWeb:DicomWebBusy");
}

using (Enter())
{
    await QueryAsync(cancellationToken);
}
```

Different Manager/facade instances and DI scopes still share the singleton registry's global native limiter. The caller owns the returned lease, just as it owns an acquired native `RateLimitLease`. Queue limit zero and default oldest-first order match the immediate native `AttemptAcquire(1)` gate; the application maps a failed attempt to its own busy exception.
