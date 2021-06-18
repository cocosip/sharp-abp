using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.DbConnectionsManagement.MongoDB
{
    [ConnectionStringName(DbConnectionsManagementDbProperties.ConnectionStringName)]
    public class DbConnectionsManagementMongoDbContext : AbpMongoDbContext, IDbConnectionsManagementMongoDbContext
    {
        public IMongoCollection<DatabaseConnectionInfo> DatabaseConnectionInfos => Collection<DatabaseConnectionInfo>();
        protected override void CreateModel(IMongoModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);
            modelBuilder.ConfigureDbConnectionsManagement();
        }
    }
}
