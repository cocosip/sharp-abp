namespace SharpAbp.Abp.Faster
{
    public interface IFasterLoggerFactory
    {
        IFasterLogger<T> GetOrCreate<T>(string name) where T : class;
    }
}
