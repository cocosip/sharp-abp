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
        private volatile MapTenancyTenant[] _tenants = [];
        protected List<MapTenancyTenant> Tenants => _tenants.Select(Copy).ToList();
        public string CacheStamp { get; set; }
        public SemaphoreSlim SyncSemaphore { get; } = new(1, 1);
        public DateTime? LastCheckTime { get; set; }

        public Task FillAsync(List<MapTenant> mapTenants)
        {
            _tenants = mapTenants.Select(x => new MapTenancyTenant(x.TenantId, x.TenantName, x.Code, x.MapCode)).ToArray();
            return Task.CompletedTask;
        }

        public virtual MapTenancyTenant GetByTenantIdOrNull(Guid tenantId)
        {
            Check.NotNull(tenantId, nameof(tenantId));
            return Copy(_tenants.FirstOrDefault(x => x.TenantId == tenantId));
        }

        public virtual MapTenancyTenant GetByCodeOrNull([NotNull] string code)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            return Copy(_tenants.FirstOrDefault(x => x.Code == code));
        }

        public virtual MapTenancyTenant GetByMapCodeOrNull([NotNull] string mapCode)
        {
            Check.NotNullOrWhiteSpace(mapCode, nameof(mapCode));
            return Copy(_tenants.FirstOrDefault(x => x.MapCode == mapCode));
        }

        public virtual IReadOnlyList<MapTenancyTenant> GetAll()
        {
            return Tenants.AsReadOnly();
        }

        private static MapTenancyTenant Copy(MapTenancyTenant tenant)
        {
            return tenant == null ? null : new MapTenancyTenant(tenant.TenantId, tenant.TenantName, tenant.Code, tenant.MapCode);
        }
    }
}
