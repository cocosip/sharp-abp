using System;
using System.Collections.Generic;
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

        public Task<TenantGroupConfiguration?> FindAsync(string normalizedName)
        {
            return Task.FromResult(Find(normalizedName));
        }

        public Task<TenantGroupConfiguration?> FindAsync(Guid id)
        {
            return Task.FromResult(Find(id));
        }

        public Task<TenantGroupConfiguration?> FindByTenantIdAsync(Guid tenantId)
        {
            return Task.FromResult(FindByTenantId(tenantId));
        }

        public Task<IReadOnlyList<TenantGroupConfiguration>> GetListAsync(bool includeDetails = false)
        {
            return Task.FromResult<IReadOnlyList<TenantGroupConfiguration>>(_options.Groups);
        }

        public TenantGroupConfiguration? Find(string normalizedName)
        {
            return _options.Groups.FirstOrDefault(t => t.NormalizedName == normalizedName);
        }

        public TenantGroupConfiguration? Find(Guid id)
        {
            return _options.Groups.FirstOrDefault(t => t.Id == id);
        }

        public TenantGroupConfiguration? FindByTenantId(Guid tenantId)
        {
            return _options.Groups.FirstOrDefault(t => t.Tenants.Contains(tenantId));
        }
    }
}
