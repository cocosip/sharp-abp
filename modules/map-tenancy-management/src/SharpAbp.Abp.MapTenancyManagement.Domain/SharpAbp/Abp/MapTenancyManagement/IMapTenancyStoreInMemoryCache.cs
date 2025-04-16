using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public interface IMapTenancyStoreInMemoryCache
    {
        string CacheStamp { get; set; }
        DateTime? LastCheckTime { get; set; }
        SemaphoreSlim SyncSemaphore { get; }
        Task FillAsync(List<MapTenant> mapTenants);
        MapTenancyTenant GetByTenantIdOrNull(Guid tenantId);
        MapTenancyTenant GetByCodeOrNull([NotNull] string code);
        MapTenancyTenant GetByMapCodeOrNull([NotNull] string mapCode);
        List<MapTenancyTenant> GetAll();
    }
}
