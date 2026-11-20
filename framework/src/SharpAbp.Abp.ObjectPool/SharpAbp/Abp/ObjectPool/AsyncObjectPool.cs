using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpAbp.Abp.ObjectPool
{
    /// <summary>
    /// A bounded async object pool that disposes items rejected by the policy or dropped
    /// because the pool is full. The pool does not block or limit concurrency: when empty,
    /// a new item is created by the policy.
    /// </summary>
    public class AsyncObjectPool<T> : IAsyncObjectPool<T>, IDisposable
        where T : class
    {
        protected IAsyncObjectPoolPolicy<T> Policy { get; }

        protected int MaximumRetained { get; }

        private readonly object _sync = new object();

        private readonly Queue<T> _items;

        private bool _disposed;

        public AsyncObjectPool(IAsyncObjectPoolPolicy<T> policy, int maximumRetained)
        {
            Policy = policy ?? throw new ArgumentNullException(nameof(policy));

            if (maximumRetained < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumRetained), "Maximum retained must be greater than or equal to zero.");
            }

            MaximumRetained = maximumRetained;
            _items = new Queue<T>(Math.Min(maximumRetained, 16));
        }

        public virtual ValueTask<T> GetAsync()
        {
            lock (_sync)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (_items.Count > 0)
                {
                    return new ValueTask<T>(_items.Dequeue());
                }
            }

            return CreateAsync();
        }

        private async ValueTask<T> CreateAsync()
        {
            var item = await Policy.CreateAsync().ConfigureAwait(false);
            lock (_sync)
            {
                if (!_disposed) return item;
            }

            PooledItemDisposer.Dispose(item);
            throw new ObjectDisposedException(GetType().Name);
        }

        public virtual void Return(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (!Policy.Return(obj))
            {
                PooledItemDisposer.Dispose(obj);
                return;
            }

            lock (_sync)
            {
                if (!_disposed && _items.Count < MaximumRetained)
                {
                    _items.Enqueue(obj);
                    return;
                }
            }

            PooledItemDisposer.Dispose(obj);
        }

        public virtual void Dispose()
        {
            List<T> items;

            lock (_sync)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                items = new List<T>(_items);
                _items.Clear();
            }

            var errors = new List<Exception>();
            foreach (var item in items)
            {
                try
                {
                    PooledItemDisposer.Dispose(item);
                }
                catch (Exception error)
                {
                    errors.Add(error);
                }
            }
            if (errors.Count > 0) throw new AggregateException(errors);
        }
    }
}
