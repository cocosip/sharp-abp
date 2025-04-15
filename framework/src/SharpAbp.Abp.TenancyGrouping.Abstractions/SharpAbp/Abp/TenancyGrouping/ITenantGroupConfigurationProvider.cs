using System.Threading.Tasks;

namespace SharpAbp.Abp.TenancyGrouping
{
    public interface ITenantGroupConfigurationProvider
    {
        Task<TenantGroupConfiguration?> GetAsync(bool saveResolveResult = false);
    }
}
