using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.TenancyGrouping.ConfigurationStore
{
    [Dependency(TryRegister = true)]
    public class DefaultTenantGroupStore : ITenantGroupStore, ITransientDependency
    {
        private readonly AbpDefaultTenantGroupStoreOptions _options;
        public DefaultTenantGroupStore(IOptionsMonitor<AbpDefaultTenantGroupStoreOptions> options)
        {
            _options = options.CurrentValue;
        }

        public Task<TenantGroupConfiguration?> FindAsync(string name)
        {
            return Task.FromResult(Find(name));
        }

        public Task<TenantGroupConfiguration?> FindAsync(Guid id)
        {
            return Task.FromResult(Find(id));
        }

        public Task<TenantGroupConfiguration?> FindByTenantIdAsync(Guid tenantId)
        {
            return Task.FromResult(FindByTenantId(tenantId));
        }

        public TenantGroupConfiguration? Find(string name)
        {
            return _options.TenantGroups?.FirstOrDefault(t => t.Name == name);
        }

        public TenantGroupConfiguration? Find(Guid id)
        {
            return _options.TenantGroups?.FirstOrDefault(t => t.Id == id);
        }

        public TenantGroupConfiguration? FindByTenantId(Guid tenantId)
        {
            return _options.TenantGroups?.FirstOrDefault(t => t.Tenants.Contains(tenantId));
        }

    }
}
