using System;

namespace SharpAbp.Abp.ObjectPool
{
    internal static class PooledItemDisposer
    {
        /// <summary>
        /// Disposes a pooled item. Synchronous disposal is preferred because it does not block
        /// the caller; asynchronous disposal is only used for items that implement
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
                asyncDisposable.DisposeAsync().AsTask().GetAwaiter().GetResult();
            }
        }
    }
}
