using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.TenancyGrouping
{
    public class UpperInvariantTenantGroupNormalizer : ITenantGroupNormalizer, ITransientDependency
    {
        public virtual string? NormalizeName(string? name)
        {
            return name?.Normalize().ToUpperInvariant();
        }
    }
}
