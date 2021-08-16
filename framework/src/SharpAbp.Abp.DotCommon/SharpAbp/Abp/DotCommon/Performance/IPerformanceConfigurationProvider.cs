using JetBrains.Annotations;

namespace SharpAbp.Abp.DotCommon.Performance
{
    public interface IPerformanceConfigurationProvider
    {
        /// <summary>
        /// Get configuration
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        PerformanceConfiguration Get([NotNull] string name);
    }
}
