using JetBrains.Annotations;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.FileStoringManagement.MongoDB
{
    public class FileStoringManagementMongoModelBuilderConfigurationOptions : AbpMongoModelBuilderConfigurationOptions
    {
        public FileStoringManagementMongoModelBuilderConfigurationOptions(
            [NotNull] string collectionPrefix = "")
            : base(collectionPrefix)
        {
        }
    }
}
