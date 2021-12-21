using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Core.Extensions
{
    public static class AbpTaskExtensions
    {
        /// <summary>
        /// 获取Task结果的扩展
        /// </summary>
        public static TResult WaitResult<TResult>(this Task<TResult> task, int timeoutMillis)
        {
            if (task.Wait(timeoutMillis))
            {
                return task.Result;
            }
            return default;
        }

        /// <summary>
        /// 设置Task过期时间
        /// </summary>
        public static async Task TimeoutAfter(this Task task, int millisecondsDelay)
        {
            var timeoutCancellationTokenSource = new CancellationTokenSource();
            var completedTask = await Task.WhenAny(task, Task.Delay(millisecondsDelay, timeoutCancellationTokenSource.Token));
            if (completedTask == task)
            {
                timeoutCancellationTokenSource.Cancel();
            }
            else
            {
                throw new TimeoutException("The operation has timed out.");
            }
        }

        /// <summary>
        /// 设置Task过期时间
        /// </summary>
        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, int millisecondsDelay)
        {
            var timeoutCancellationTokenSource = new CancellationTokenSource();
            var completedTask = await Task.WhenAny(task, Task.Delay(millisecondsDelay, timeoutCancellationTokenSource.Token));
            if (completedTask == task)
            {
                timeoutCancellationTokenSource.Cancel();
                return task.Result;
            }
            throw new TimeoutException("The operation has timed out.");
        }
    }
}