using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public interface ITenantGroupCacheManager
    {
        /// <summary>
        /// Removes tenant group cache entries by ID and/or normalized name.
        /// </summary>
        /// <param name="id">The tenant group ID.</param>
        /// <param name="normalizedName">The normalized name of the tenant group.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="AbpException">Thrown when both id and normalizedName are invalid.</exception>
        Task RemoveAsync(Guid? id, string normalizedName, CancellationToken cancellationToken = default);
    }
}
