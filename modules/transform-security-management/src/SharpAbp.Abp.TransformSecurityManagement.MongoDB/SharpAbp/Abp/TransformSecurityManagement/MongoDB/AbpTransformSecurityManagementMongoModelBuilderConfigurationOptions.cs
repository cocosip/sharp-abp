using JetBrains.Annotations;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.TransformSecurityManagement.MongoDB
{
    public class AbpTransformSecurityManagementMongoModelBuilderConfigurationOptions : AbpMongoModelBuilderConfigurationOptions
    {
        public AbpTransformSecurityManagementMongoModelBuilderConfigurationOptions([NotNull] string collectionPrefix = "")
            : base(collectionPrefix)
        {

        }
    }
}
