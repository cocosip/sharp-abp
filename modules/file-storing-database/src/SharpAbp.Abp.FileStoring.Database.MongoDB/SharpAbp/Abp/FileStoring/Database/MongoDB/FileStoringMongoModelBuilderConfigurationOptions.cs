using JetBrains.Annotations;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.FileStoring.Database.MongoDB
{
    public class FileStoringMongoModelBuilderConfigurationOptions : AbpMongoModelBuilderConfigurationOptions
    {
        public FileStoringMongoModelBuilderConfigurationOptions(
            [NotNull] string collectionPrefix = "")
            : base(collectionPrefix)
        {
        }
    }
}
