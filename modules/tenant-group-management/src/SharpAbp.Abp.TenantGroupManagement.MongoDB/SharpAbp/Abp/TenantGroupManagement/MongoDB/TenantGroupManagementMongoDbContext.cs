using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.TenantGroupManagement.MongoDB
{
    [ConnectionStringName(TenantGroupManagementDbProperties.ConnectionStringName)]
    public class TenantGroupManagementMongoDbContext : AbpMongoDbContext, ITenantGroupManagementMongoDbContext
    {
        public IMongoCollection<TenantGroup> TenantGroups => Collection<TenantGroup>();
        public IMongoCollection<TenantGroupTenant> TenantGroupTenants => Collection<TenantGroupTenant>();
        public IMongoCollection<TenantGroupConnectionString> TenantGroupConnectionStrings => Collection<TenantGroupConnectionString>();

        protected override void CreateModel(IMongoModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);
            modelBuilder.ConfigureTenantGroupManagement();
        }
    }
}
