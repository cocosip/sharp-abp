using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.TransformSecurityManagement.MongoDB
{
    [ConnectionStringName(AbpTransformSecurityManagementDbProperties.ConnectionStringName)]
    public class AbpTransformSecurityManagementMongoDbContext : AbpMongoDbContext, IAbpTransformSecurityManagementMongoDbContext
    {
        public IMongoCollection<SecurityCredentialInfo> SecurityCredentialInfos => Collection<SecurityCredentialInfo>();

        protected override void CreateModel(IMongoModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);
            modelBuilder.ConfigureTransformSecurityManagement();
        }
    }
}
