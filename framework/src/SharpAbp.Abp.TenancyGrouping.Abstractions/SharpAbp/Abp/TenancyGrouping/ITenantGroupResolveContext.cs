using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.TenancyGrouping
{
    public interface ITenantGroupResolveContext : IServiceProviderAccessor
    {
        string? GroupIdOrName { get; set; }

        bool Handled { get; set; }
    }
}
