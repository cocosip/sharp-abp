using System.Collections.Generic;
using JetBrains.Annotations;

namespace SharpAbp.Abp.TenancyGrouping
{
    public class AbpTenantGroupResolveOptions
    {
        [NotNull]
        public List<ITenantGroupResolveContributor> TenantResolvers { get; }

        public AbpTenantGroupResolveOptions()
        {
            TenantResolvers = [];
        }
    }
}
