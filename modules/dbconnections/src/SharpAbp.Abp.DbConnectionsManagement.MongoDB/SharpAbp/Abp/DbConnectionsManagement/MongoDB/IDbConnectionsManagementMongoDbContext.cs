using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.DbConnectionsManagement.MongoDB
{
    [ConnectionStringName(DbConnectionsManagementDbProperties.ConnectionStringName)]
    public interface IDbConnectionsManagementMongoDbContext : IAbpMongoDbContext
    {
        IMongoCollection<DatabaseConnectionInfo> DatabaseConnectionInfos { get; }
    }
}
