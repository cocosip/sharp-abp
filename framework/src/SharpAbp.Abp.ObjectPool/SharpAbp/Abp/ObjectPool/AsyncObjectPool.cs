using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.ObjectPool
{
    public class AsyncObjectPool<T> : IAsyncObjectPool<T>
        where T : class
    {
        protected IAsyncObjectPoolPolicy<T> Policy { get; }

        protected ConcurrentQueue<T> Items { get; }

        protected int MaximumRetained { get; }

        private int _retained;

        public AsyncObjectPool(IAsyncObjectPoolPolicy<T> policy, int maximumRetained)
        {
            Policy = policy;
            Items = new ConcurrentQueue<T>();
            MaximumRetained = maximumRetained;
        }

        public virtual ValueTask<T> GetAsync()
        {
            if (Items.TryDequeue(out var item))
            {
                Interlocked.Decrement(ref _retained);
                return new ValueTask<T>(item);
            }

            return Policy.CreateAsync();
        }

        public virtual void Return(T obj)
        {
            if (!Policy.Return(obj))
            {
                return;
            }

            if (Interlocked.Increment(ref _retained) <= MaximumRetained)
            {
                Items.Enqueue(obj);
                return;
            }

            Interlocked.Decrement(ref _retained);
        }
    }
}
