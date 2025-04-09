using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Core.Extensions
{
    /// <summary>
    /// TaskFactory extensions
    /// </summary>
    public static class AbpTaskFactoryExtensions
    {
        /// <summary>
        /// 延迟一定时间后开启任务
        /// </summary>
        /// <param name="factory">TaskFactory</param>
        /// <param name="millisecondsDelay">延迟毫秒数</param>
        /// <param name="action">执行方法委托函数</param>
        /// <returns></returns>
        public static Task StartDelayedTask(this TaskFactory factory, int millisecondsDelay, Action action)
        {
            // Validate arguments
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
            if (millisecondsDelay < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsDelay));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            // Check for a pre-canceled token
            if (factory.CancellationToken.IsCancellationRequested)
            {
                return new Task(() => { }, factory.CancellationToken);
            }

            // Create the timed task
            var tcs = new TaskCompletionSource<object?>(factory.CreationOptions);
            var ctr = default(CancellationTokenRegistration);

            // Create the timer but don't start it yet.  If we start it now,
            // it might fire before ctr has been set to the right registration.
            var ctr1 = ctr;
            var timer = new Timer(self =>
            {
                // Clean up both the cancellation token and the timer, and try to transition to completed
                ctr1.Dispose();
                ((Timer)self).Dispose();
                tcs.TrySetResult(null);
            }, null, -1, -1);

            // Register with the cancellation token.
            if (factory.CancellationToken.CanBeCanceled)
            {
                // When cancellation occurs, cancel the timer and try to transition to canceled.
                // There could be a race, but it's benign.
                factory.CancellationToken.Register(() =>
                {
                    timer.Dispose();
                    tcs.TrySetCanceled();
                });
            }

            // Start the timer and hand back the task...
            try
            {
                timer.Change(millisecondsDelay, Timeout.Infinite);
            }
            catch (ObjectDisposedException)
            {
                // in case there's a race with cancellation; this is benign
            }

            return tcs.Task.ContinueWith(_ => action(), factory.CancellationToken, TaskContinuationOptions.OnlyOnRanToCompletion, factory.Scheduler ?? TaskScheduler.Current);
        }
    }
}