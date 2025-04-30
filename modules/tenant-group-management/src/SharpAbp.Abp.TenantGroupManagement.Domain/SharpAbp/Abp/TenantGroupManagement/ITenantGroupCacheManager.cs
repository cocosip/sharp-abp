using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public interface ITenantGroupCacheManager
    {
        Task RemoveAsync(Guid? id, string normalizedName, CancellationToken cancellationToken = default);
    }
}
