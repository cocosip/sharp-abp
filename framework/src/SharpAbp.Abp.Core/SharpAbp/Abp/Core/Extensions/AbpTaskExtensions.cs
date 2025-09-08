using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Core.Extensions
{
    /// <summary>
    /// Extension methods for Task operations providing timeout and result waiting functionality
    /// </summary>
    public static class AbpTaskExtensions
    {
        /// <summary>
        /// Waits for the task to complete within the specified timeout and returns the result
        /// </summary>
        /// <typeparam name="TResult">The type of the task result</typeparam>
        /// <param name="task">The task to wait for</param>
        /// <param name="timeoutMillis">The timeout in milliseconds</param>
        /// <returns>The task result if completed within timeout, otherwise the default value of TResult</returns>
        /// <exception cref="ArgumentNullException">Thrown when task is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when timeoutMillis is negative</exception>
        public static TResult WaitResult<TResult>(this Task<TResult> task, int timeoutMillis)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));
            if (timeoutMillis < 0)
                throw new ArgumentOutOfRangeException(nameof(timeoutMillis), "Timeout cannot be negative");

            if (task.Wait(timeoutMillis))
            {
                return task.Result;
            }
            return default!;
        }

        /// <summary>
        /// Adds a timeout to the specified task. The task will be cancelled if it doesn't complete within the specified time
        /// </summary>
        /// <param name="task">The task to add timeout to</param>
        /// <param name="millisecondsDelay">The timeout delay in milliseconds</param>
        /// <returns>A task that completes when the original task completes or throws TimeoutException if timeout occurs</returns>
        /// <exception cref="ArgumentNullException">Thrown when task is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when millisecondsDelay is negative</exception>
        /// <exception cref="TimeoutException">Thrown when the task doesn't complete within the specified timeout</exception>
        public static async Task TimeoutAfter(this Task task, int millisecondsDelay)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));
            if (millisecondsDelay < 0)
                throw new ArgumentOutOfRangeException(nameof(millisecondsDelay), "Delay cannot be negative");

            using var timeoutCancellationTokenSource = new CancellationTokenSource();
            var completedTask = await Task.WhenAny(task, Task.Delay(millisecondsDelay, timeoutCancellationTokenSource.Token)).ConfigureAwait(false);
            
            if (completedTask == task)
            {
                timeoutCancellationTokenSource.Cancel();
                await task.ConfigureAwait(false); // Propagate any exceptions from the original task
            }
            else
            {
                throw new TimeoutException("The operation has timed out.");
            }
        }

        /// <summary>
        /// Adds a timeout to the specified task with result. The task will be cancelled if it doesn't complete within the specified time
        /// </summary>
        /// <typeparam name="TResult">The type of the task result</typeparam>
        /// <param name="task">The task to add timeout to</param>
        /// <param name="millisecondsDelay">The timeout delay in milliseconds</param>
        /// <returns>A task that returns the result when the original task completes or throws TimeoutException if timeout occurs</returns>
        /// <exception cref="ArgumentNullException">Thrown when task is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when millisecondsDelay is negative</exception>
        /// <exception cref="TimeoutException">Thrown when the task doesn't complete within the specified timeout</exception>
        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, int millisecondsDelay)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));
            if (millisecondsDelay < 0)
                throw new ArgumentOutOfRangeException(nameof(millisecondsDelay), "Delay cannot be negative");

            using var timeoutCancellationTokenSource = new CancellationTokenSource();
            var completedTask = await Task.WhenAny(task, Task.Delay(millisecondsDelay, timeoutCancellationTokenSource.Token)).ConfigureAwait(false);
            
            if (completedTask == task)
            {
                timeoutCancellationTokenSource.Cancel();
                return await task.ConfigureAwait(false); // Propagate any exceptions and return the result
            }
            
            throw new TimeoutException("The operation has timed out.");
        }
    }
}