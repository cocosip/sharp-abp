using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenancyStoreInMemoryCache : IMapTenancyStoreInMemoryCache, ISingletonDependency
    {

        public string CacheStamp { get; set; }
        protected List<MapTenancyTenant> Tenants { get; }
        public SemaphoreSlim SyncSemaphore { get; } = new(1, 1);

        public DateTime? LastCheckTime { get; set; }

        public MapTenancyStoreInMemoryCache()
        {
            Tenants = [];
        }

        public Task FillAsync(List<MapTenant> mapTenants)
        {
            Tenants.Clear();
            var tenants = mapTenants.Select(x => new MapTenancyTenant(x.TenantId, x.TenantName, x.Code, x.MapCode)).ToList();
            Tenants.AddRange(tenants);
            return Task.CompletedTask;
        }

        public virtual MapTenancyTenant GetByTenantIdOrNull(Guid tenantId)
        {
            Check.NotNull(tenantId, nameof(tenantId));
            return Tenants.FirstOrDefault(x => x.TenantId == tenantId);
        }

        public virtual MapTenancyTenant GetByCodeOrNull([NotNull] string code)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            return Tenants.FirstOrDefault(x => x.Code == code);
        }

        public virtual MapTenancyTenant GetByMapCodeOrNull([NotNull] string mapCode)
        {
            Check.NotNullOrWhiteSpace(mapCode, nameof(mapCode));
            return Tenants.FirstOrDefault(x => x.MapCode == mapCode);
        }

        public virtual List<MapTenancyTenant> GetAll()
        {
            return Tenants;
        }

    }
}
