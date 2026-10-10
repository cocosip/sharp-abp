using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    public class ConcurrencyLimitingManager : IConcurrencyLimitingManager, ITransientDependency
    {
        protected IConcurrencyLimitingPolicyProvider Policies { get; }
        protected IConcurrencyLimitingPartitionResolver Resolver { get; }
        protected IConcurrencyLimiterRegistry Registry { get; }
        protected bool IsEnabled { get; }
        public ConcurrencyLimitingManager(
            IConcurrencyLimitingPolicyProvider policies,
            IConcurrencyLimitingPartitionResolver resolver,
            IConcurrencyLimiterRegistry registry,
            IConcurrencyLimitingConfiguration configuration)
        {
            Policies = policies;
            Resolver = resolver;
            Registry = registry;
            IsEnabled = configuration.IsEnabled;
        }

        public virtual ConcurrencyLimitingLease TryAcquire(string policyName, ConcurrencyLimitingContext? context = null)
        {
            SharpAbpConcurrencyLimitingOptions.ValidateName(policyName, nameof(policyName));
            if (!IsEnabled)
            {
                return new ConcurrencyLimitingLease(null);
            }
            var policy = Policies.Get(policyName);
            var key = Resolver.Resolve(policy, context ?? new ConcurrencyLimitingContext());
            return new ConcurrencyLimitingLease(Registry.Get(policy).AttemptAcquire(key, 1));
        }

        public virtual async Task<ConcurrencyLimitingLease> AcquireAsync(
            string policyName,
            ConcurrencyLimitingContext? context = null,
            CancellationToken cancellationToken = default)
        {
            SharpAbpConcurrencyLimitingOptions.ValidateName(policyName, nameof(policyName));
            cancellationToken.ThrowIfCancellationRequested();
            if (!IsEnabled)
            {
                return new ConcurrencyLimitingLease(null);
            }
            var policy = Policies.Get(policyName);
            var key = Resolver.Resolve(policy, context ?? new ConcurrencyLimitingContext());
            var limiter = Registry.Get(policy);
            using (var timeout = new CancellationTokenSource(policy.QueueTimeout))
            using (var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token))
            {
                System.Threading.RateLimiting.RateLimitLease? lease = null;
                try
                {
                    lease = await limiter.AcquireAsync(key, 1, linked.Token).ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();
                    if (timeout.IsCancellationRequested)
                    {
                        throw new ConcurrencyLimitExceededException(policy, ConcurrencyLimitExceededReason.QueueTimeout);
                    }
                    if (!lease.IsAcquired)
                    {
                        throw new ConcurrencyLimitExceededException(policy, policy.QueueLimit == 0 ? ConcurrencyLimitExceededReason.Capacity : ConcurrencyLimitExceededReason.QueueFull);
                    }
                    var result = new ConcurrencyLimitingLease(lease);
                    lease = null; // The caller now owns this permit.
                    return result;
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested && timeout.IsCancellationRequested)
                {
                    throw new ConcurrencyLimitExceededException(policy, ConcurrencyLimitExceededReason.QueueTimeout);
                }
                finally
                {
                    // Includes cancellation racing a native successful acquisition.
                    lease?.Dispose();
                }
            }
        }

        public virtual ConcurrencyLimitingStatistics GetStatistics(string policyName, ConcurrencyLimitingContext? context = null)
        {
            SharpAbpConcurrencyLimitingOptions.ValidateName(policyName, nameof(policyName));
            if (!IsEnabled)
            {
                return new ConcurrencyLimitingStatistics(false, int.MaxValue, 0);
            }
            var policy = Policies.Get(policyName);
            var key = Resolver.Resolve(policy, context ?? new ConcurrencyLimitingContext());
            var statistics = Registry.Get(policy).GetStatistics(key)!;
            return new ConcurrencyLimitingStatistics(true, statistics.CurrentAvailablePermits, statistics.CurrentQueuedCount);
        }

        public virtual async Task ExecuteAsync(
            string policyName,
            Func<CancellationToken, Task> operation,
            ConcurrencyLimitingContext? context = null,
            CancellationToken cancellationToken = default)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }
            using (await AcquireAsync(policyName, context, cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();
                await operation(cancellationToken).ConfigureAwait(false);
            }
        }

        public virtual async Task<T> ExecuteAsync<T>(
            string policyName,
            Func<CancellationToken, Task<T>> operation,
            ConcurrencyLimitingContext? context = null,
            CancellationToken cancellationToken = default)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }
            using (await AcquireAsync(policyName, context, cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();
                return await operation(cancellationToken).ConfigureAwait(false);
            }
        }
    }

}
