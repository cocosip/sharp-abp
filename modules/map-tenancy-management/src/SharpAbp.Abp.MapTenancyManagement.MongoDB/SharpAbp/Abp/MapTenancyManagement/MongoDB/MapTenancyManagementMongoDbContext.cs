using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.MapTenancyManagement.MongoDB
{
    [ConnectionStringName(MapTenancyManagementDbProperties.ConnectionStringName)]
    public class MapTenancyManagementMongoDbContext : AbpMongoDbContext, IMapTenancyManagementMongoDbContext
    {
        public IMongoCollection<MapTenant> MapTenants => Collection<MapTenant>();

        protected override void CreateModel(IMongoModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);

            modelBuilder.ConfigureMapTenancyManagement();
        }
    }
}