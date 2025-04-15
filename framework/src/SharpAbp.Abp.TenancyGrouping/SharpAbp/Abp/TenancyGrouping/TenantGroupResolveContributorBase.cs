using System.Threading.Tasks;

namespace SharpAbp.Abp.TenancyGrouping
{
    public abstract class TenantGroupResolveContributorBase : ITenantGroupResolveContributor
    {
        public abstract string Name { get; }
        public abstract Task ResolveAsync(ITenantGroupResolveContext context);
    }
}
