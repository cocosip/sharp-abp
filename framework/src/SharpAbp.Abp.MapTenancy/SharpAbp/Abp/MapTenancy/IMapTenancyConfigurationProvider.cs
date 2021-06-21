using JetBrains.Annotations;
using System.Threading.Tasks;

namespace SharpAbp.Abp.MapTenancy
{
    public interface IMapTenancyConfigurationProvider
    {
        /// <summary>
        /// Get by LocalSystem code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<MapTenancyConfiguration> GetAsync([NotNull] string code);

        /// <summary>
        /// Get by third part system mapCode
        /// </summary>
        /// <param name="mapCode"></param>
        /// <returns></returns>
        Task<MapTenancyConfiguration> GetByMapCodeAsync([NotNull] string mapCode);
    }
}
