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
        protected List<MapTenancyTenant> Tenants { get; } = [];
        public string CacheStamp { get; set; }
        public SemaphoreSlim SyncSemaphore { get; } = new(1, 1);
        public DateTime? LastCheckTime { get; set; }

        public Task FillAsync(List<MapTenant> mapTenants)
        {
            var tenants = mapTenants.Select(x => new MapTenancyTenant(x.TenantId, x.TenantName, x.Code, x.MapCode)).ToList();
            lock (Tenants)
            {
                Tenants.Clear();
                Tenants.AddRange(tenants);
            }
            return Task.CompletedTask;
        }

        public virtual MapTenancyTenant GetByTenantIdOrNull(Guid tenantId)
        {
            Check.NotNull(tenantId, nameof(tenantId));
            lock (Tenants)
            {
                return Copy(Tenants.FirstOrDefault(x => x.TenantId == tenantId));
            }
        }

        public virtual MapTenancyTenant GetByCodeOrNull([NotNull] string code)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            lock (Tenants)
            {
                return Copy(Tenants.FirstOrDefault(x => x.Code == code));
            }
        }

        public virtual MapTenancyTenant GetByMapCodeOrNull([NotNull] string mapCode)
        {
            Check.NotNullOrWhiteSpace(mapCode, nameof(mapCode));
            lock (Tenants)
            {
                return Copy(Tenants.FirstOrDefault(x => x.MapCode == mapCode));
            }
        }

        public virtual IReadOnlyList<MapTenancyTenant> GetAll()
        {
            lock (Tenants)
            {
                return Tenants.Select(Copy).ToList().AsReadOnly();
            }
        }

        private static MapTenancyTenant Copy(MapTenancyTenant tenant)
        {
            return tenant == null ? null : new MapTenancyTenant(tenant.TenantId, tenant.TenantName, tenant.Code, tenant.MapCode);
        }
    }
}
