using JetBrains.Annotations;
using System.Threading.Tasks;

namespace SharpAbp.Abp.MapTenancy
{
    public interface IMapTenancyConfigurationProvider
    {
        Task<MapTenancyConfiguration> GetAsync([NotNull] string code);
    }
}
