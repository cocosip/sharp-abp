using System;

namespace SharpAbp.Abp.TenancyGrouping
{
    public class TenantGroupResolveContext : ITenantGroupResolveContext
    {
        public IServiceProvider ServiceProvider { get; }

        public string? GroupIdOrName { get; set; }

        public bool Handled { get; set; }

        public bool HasResolvedTenantOrHost()
        {
            return Handled || GroupIdOrName != null;
        }

        public TenantGroupResolveContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}
