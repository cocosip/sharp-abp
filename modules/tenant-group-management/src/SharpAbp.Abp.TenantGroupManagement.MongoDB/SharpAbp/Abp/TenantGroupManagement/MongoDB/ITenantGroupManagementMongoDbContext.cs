using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.TenantGroupManagement.MongoDB
{
    [ConnectionStringName(TenantGroupManagementDbProperties.ConnectionStringName)]
    public interface ITenantGroupManagementMongoDbContext : IAbpMongoDbContext
    {
        IMongoCollection<TenantGroup> TenantGroups { get; }
        IMongoCollection<TenantGroupTenant> TenantGroupTenants { get; }
        IMongoCollection<TenantGroupConnectionString> TenantGroupConnectionStrings { get; }

    }
}
