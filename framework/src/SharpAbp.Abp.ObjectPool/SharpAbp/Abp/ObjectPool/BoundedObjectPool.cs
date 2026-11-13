using System;
using System.Collections.Generic;
using Microsoft.Extensions.ObjectPool;

namespace SharpAbp.Abp.ObjectPool
{
    /// <summary>
    /// A bounded object pool that disposes items rejected by the policy or dropped because
    /// the pool is full. Unlike <see cref="DefaultObjectPool{T}"/>, an overflow item is not
    /// silently discarded, which matters for pools holding disposable SDK clients.
    /// </summary>
    public class BoundedObjectPool<T> : ObjectPool<T>, IDisposable
        where T : class
    {
        protected IPooledObjectPolicy<T> Policy { get; }

        protected int MaximumRetained { get; }

        private readonly object _sync = new object();

        private readonly Queue<T> _items;

        private bool _disposed;

        public BoundedObjectPool(IPooledObjectPolicy<T> policy, int maximumRetained)
        {
            Policy = policy ?? throw new ArgumentNullException(nameof(policy));

            if (maximumRetained < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumRetained), "Maximum retained must be greater than or equal to zero.");
            }

            MaximumRetained = maximumRetained;
            _items = new Queue<T>(Math.Min(maximumRetained, 16));
        }

        public override T Get()
        {
            lock (_sync)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (_items.Count > 0)
                {
                    return _items.Dequeue();
                }

                return Policy.Create();
            }
        }

        public override void Return(T obj)
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

            foreach (var item in items)
            {
                PooledItemDisposer.Dispose(item);
            }
        }
    }
}
