namespace SharpAbp.Abp.DotCommon.Performance
{
    public interface IPerformanceService
    {
        /// <summary>
        /// Start
        /// </summary>
        void Start();

        /// <summary>
        /// Stop
        /// </summary>
        void Stop();

        /// <summary>
        /// Increament key count
        /// </summary>
        /// <param name="key"></param>
        /// <param name="rtMilliseconds"></param>
        void IncrementKeyCount(string key, double rtMilliseconds);

        /// <summary>
        /// Update key count
        /// </summary>
        /// <param name="key"></param>
        /// <param name="count"></param>
        /// <param name="rtMilliseconds"></param>
        void UpdateKeyCount(string key, long count, double rtMilliseconds);

        /// <summary>
        /// Get key performanceInfo
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        PerformanceInfo GetKeyPerformanceInfo(string key);
    
    }
}
