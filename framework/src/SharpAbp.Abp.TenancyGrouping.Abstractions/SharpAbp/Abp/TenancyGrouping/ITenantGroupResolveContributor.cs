using System.Threading.Tasks;

namespace SharpAbp.Abp.TenancyGrouping
{
    public interface ITenantGroupResolveContributor
    {
        string Name { get; }

        Task ResolveAsync(ITenantGroupResolveContext context);
    }
}
