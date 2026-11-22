using System;

using Volo.Abp.Threading;

namespace SharpAbp.Abp.ObjectPool
{
    internal static class PooledItemDisposer
    {
        /// <summary>
        /// Disposes a pooled item. Synchronous disposal is preferred; asynchronous disposal
        /// runs through ABP's synchronous bridge for items that implement
        /// <see cref="IAsyncDisposable"/> exclusively.
        /// </summary>
        public static void Dispose(object item)
        {
            if (item is IDisposable disposable)
            {
                disposable.Dispose();
                return;
            }

            if (item is IAsyncDisposable asyncDisposable)
            {
                AsyncHelper.RunSync(() => asyncDisposable.DisposeAsync().AsTask());
            }
        }
    }
}
