using JetBrains.Annotations;
using Volo.Abp.MongoDB;

namespace SharpAbp.MinId.MongoDB
{
    public class MinIdMongoModelBuilderConfigurationOptions : AbpMongoModelBuilderConfigurationOptions
    {
        public MinIdMongoModelBuilderConfigurationOptions(
            [NotNull] string collectionPrefix = "")
            : base(collectionPrefix)
        {
        }
    }
}