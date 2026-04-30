using System.Threading.Tasks;

namespace SharpAbp.Abp.ObjectPool
{
    public interface IAsyncObjectPoolPolicy<T>
        where T : class
    {
        ValueTask<T> CreateAsync();

        bool Return(T obj);
    }
}
