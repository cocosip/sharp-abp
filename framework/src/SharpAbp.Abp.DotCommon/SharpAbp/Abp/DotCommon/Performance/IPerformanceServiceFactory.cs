using JetBrains.Annotations;

namespace SharpAbp.Abp.DotCommon.Performance
{
    public interface IPerformanceServiceFactory
    {
        /// <summary>
        /// GetOrCreate
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IPerformanceService GetOrCreate([NotNull] string name);

        /// <summary>
        /// Stop all the performance
        /// </summary>
        void StopAll();
    }
}
