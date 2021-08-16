namespace SharpAbp.Abp.DotCommon.Performance
{
    public interface ICountInfoFactory
    {
        /// <summary>
        /// Create countInfo
        /// </summary>
        /// <param name="name"></param>
        /// <param name="key"></param>
        /// <param name="configuration"></param>
        /// <param name="initialCount"></param>
        /// <param name="rtMilliseconds"></param>
        /// <returns></returns>
        CountInfo Create(string name, string key, PerformanceConfiguration configuration, long initialCount, double rtMilliseconds);
    }
}
