using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    /// <summary>Acquires process-local execution permits in the current application container.</summary>
    public interface IConcurrencyLimitingManager
    {
        /// <summary>Attempts one permit immediately without entering the queue. Dispose the returned lease.</summary>
        ConcurrencyLimitingLease TryAcquire(string policyName, ConcurrencyLimitingContext? context = null);
        /// <summary>Acquires one permit, with bounded policy queuing. Caller cancellation remains cancellation.</summary>
        Task<ConcurrencyLimitingLease> AcquireAsync(string policyName, ConcurrencyLimitingContext? context = null, CancellationToken cancellationToken = default);
        /// <summary>Returns a non-reserving snapshot of the partition's current capacity and queue.</summary>
        ConcurrencyLimitingStatistics GetStatistics(string policyName, ConcurrencyLimitingContext? context = null);
        /// <summary>Holds a permit until the operation completes and always releases it.</summary>
        Task ExecuteAsync(string policyName, Func<CancellationToken, Task> operation, ConcurrencyLimitingContext? context = null, CancellationToken cancellationToken = default);
        /// <summary>Holds a permit until the result-producing operation completes and always releases it.</summary>
        Task<T> ExecuteAsync<T>(string policyName, Func<CancellationToken, Task<T>> operation, ConcurrencyLimitingContext? context = null, CancellationToken cancellationToken = default);
    }

}
