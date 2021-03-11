using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.MapTenancyManagement.MongoDB
{
    [ConnectionStringName(MapTenancyManagementDbProperties.ConnectionStringName)]
    public interface IMapTenancyManagementMongoDbContext : IAbpMongoDbContext
    {
        IMongoCollection<MapTenant> MapTenants { get; }
    }
}
