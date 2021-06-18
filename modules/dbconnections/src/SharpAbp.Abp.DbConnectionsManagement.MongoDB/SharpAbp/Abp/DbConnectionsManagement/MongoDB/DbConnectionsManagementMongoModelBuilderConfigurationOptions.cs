using JetBrains.Annotations;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.DbConnectionsManagement.MongoDB
{
    public class DbConnectionsManagementMongoModelBuilderConfigurationOptions : AbpMongoModelBuilderConfigurationOptions
    {
        public DbConnectionsManagementMongoModelBuilderConfigurationOptions([NotNull] string collectionPrefix = "") 
            : base(collectionPrefix)
        {

        }
    }
}
