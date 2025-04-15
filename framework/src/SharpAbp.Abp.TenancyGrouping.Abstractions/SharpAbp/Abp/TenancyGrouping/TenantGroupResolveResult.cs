using System.Collections.Generic;

namespace SharpAbp.Abp.TenancyGrouping
{
    public class TenantGroupResolveResult
    {
        public string? GroupIdOrName { get; set; }

        public List<string> AppliedResolvers { get; }

        public TenantGroupResolveResult()
        {
            AppliedResolvers = [];
        }
    }
}
