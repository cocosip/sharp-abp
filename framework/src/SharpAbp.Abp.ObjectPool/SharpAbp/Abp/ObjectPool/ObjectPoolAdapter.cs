using Microsoft.Extensions.ObjectPool;

namespace SharpAbp.Abp.ObjectPool
{
    public class ObjectPoolAdapter<T> : IObjectPool<T>
        where T : class
    {
        protected ObjectPool<T> Pool { get; }

        public ObjectPoolAdapter(ObjectPool<T> pool)
        {
            Pool = pool;
        }

        public virtual T Get()
        {
            return Pool.Get();
        }

        public virtual void Return(T obj)
        {
            Pool.Return(obj);
        }
    }
}
