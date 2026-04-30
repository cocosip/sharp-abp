using System.Threading.Tasks;

namespace SharpAbp.Abp.ObjectPool
{
    public interface IAsyncObjectPool<T>
        where T : class
    {
        ValueTask<T> GetAsync();

        void Return(T obj);
    }
}
