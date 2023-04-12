using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public interface ITenantGroupCacheManager
    {
        Task UpdateTenantGroupCacheAsync(Guid id, CancellationToken cancellationToken = default);
        Task RemoveTenantGroupCacheAsync(Guid id, string name, List<Guid> tenantIds, CancellationToken cancellationToken = default);
    }
}
