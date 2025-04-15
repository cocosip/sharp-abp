using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SharpAbp.Abp.TenancyGrouping
{
    public interface ITenantGroupResolver
    {
        /// <summary>
        /// Tries to resolve current tenant using registered <see cref="ITenantGroupResolveContributor"/> implementations.
        /// </summary>
        /// <returns>
        /// Tenant id, unique name or null (if could not resolve).
        /// </returns>
        [NotNull]
        Task<TenantGroupResolveResult> ResolveGroupIdOrNameAsync();
    }
}
