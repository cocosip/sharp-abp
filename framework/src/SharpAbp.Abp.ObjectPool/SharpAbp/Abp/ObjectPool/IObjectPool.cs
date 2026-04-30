namespace SharpAbp.Abp.ObjectPool
{
    public interface IObjectPool<T>
        where T : class
    {
        T Get();

        void Return(T obj);
    }
}
