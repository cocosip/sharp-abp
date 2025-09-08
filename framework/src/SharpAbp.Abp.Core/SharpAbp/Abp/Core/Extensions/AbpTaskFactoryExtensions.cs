using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Core.Extensions
{
    /// <summary>
    /// Extension methods for TaskFactory providing delayed task execution functionality
    /// </summary>
    public static class AbpTaskFactoryExtensions
    {
        /// <summary>
        /// Starts a task after a specified delay using the provided TaskFactory
        /// </summary>
        /// <param name="factory">The TaskFactory instance to use for task creation</param>
        /// <param name="millisecondsDelay">The delay in milliseconds before starting the task</param>
        /// <param name="action">The action to execute after the delay</param>
        /// <returns>A Task that represents the delayed execution of the action</returns>
        /// <exception cref="ArgumentNullException">Thrown when factory or action is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when millisecondsDelay is negative</exception>
        /// <remarks>
        /// This method creates a delayed task that respects the TaskFactory's cancellation token and scheduler.
        /// If the cancellation token is already cancelled, the task will be created in a cancelled state.
        /// The timer is properly disposed of when the task completes or is cancelled.
        /// </remarks>
        public static Task StartDelayedTask(this TaskFactory factory, int millisecondsDelay, Action action)
        {
            // Validate arguments
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            if (millisecondsDelay < 0)
                throw new ArgumentOutOfRangeException(nameof(millisecondsDelay), "Delay cannot be negative");
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            // Check for a pre-canceled token
            if (factory.CancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(factory.CancellationToken);
            }

            // Create the timed task
            var tcs = new TaskCompletionSource<object?>(factory.CreationOptions);
            var ctr = default(CancellationTokenRegistration);

            // Create the timer but don't start it yet to avoid race conditions
            // with cancellation token registration
            var timer = new Timer(self =>
            {
                // Clean up both the cancellation token registration and the timer,
                // then try to transition the task to completed state
                ctr.Dispose();
                ((Timer)self!).Dispose();
                tcs.TrySetResult(null);
            }, null, Timeout.Infinite, Timeout.Infinite);

            // Register with the cancellation token if it supports cancellation
            if (factory.CancellationToken.CanBeCanceled)
            {
                // When cancellation occurs, dispose the timer and try to transition to cancelled state
                // There could be a race condition, but it's handled gracefully
                ctr = factory.CancellationToken.Register(() =>
                {
                    timer.Dispose();
                    tcs.TrySetCanceled(factory.CancellationToken);
                });
            }

            // Start the timer with the specified delay
            try
            {
                timer.Change(millisecondsDelay, Timeout.Infinite);
            }
            catch (ObjectDisposedException)
            {
                // Handle race condition with cancellation - this is benign
                // The task will be properly cancelled through the cancellation token
            }

            // Return a continuation task that executes the action when the delay completes
            return tcs.Task.ContinueWith(
                _ => action(),
                factory.CancellationToken,
                TaskContinuationOptions.OnlyOnRanToCompletion,
                factory.Scheduler ?? TaskScheduler.Current);
        }
    }
}