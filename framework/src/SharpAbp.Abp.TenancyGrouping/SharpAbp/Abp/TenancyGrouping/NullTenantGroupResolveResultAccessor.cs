using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.TenancyGrouping
{
    public class NullTenantGroupResolveResultAccessor : ITenantGroupResolveResultAccessor, ISingletonDependency
    {
        public TenantGroupResolveResult? Result { get => null; set { } }
    }
}
