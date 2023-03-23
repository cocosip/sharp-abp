using JetBrains.Annotations;
using System.Threading.Tasks;

namespace SharpAbp.Abp.TenancyGrouping
{
    public interface ITenantGroupConfigurationProvider
    {
        Task<TenantGroupConfiguration> GetAsync([NotNull] string tenantGroupIdOrName);
    }
}
