using JetBrains.Annotations;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.TenantGroupManagement.MongoDB
{
    public class TenantGroupManagementMongoModelBuilderConfigurationOptions : AbpMongoModelBuilderConfigurationOptions
    {
        public TenantGroupManagementMongoModelBuilderConfigurationOptions([NotNull] string collectionPrefix = "")
            : base(collectionPrefix)
        {

        }
    }
}
