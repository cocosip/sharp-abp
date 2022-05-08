using System;
using Volo.Abp;
using Volo.Abp.MongoDB;

namespace SharpAbp.MinId.MongoDB
{
    public static class MinIdMongoDbContextExtensions
    {
        public static void ConfigureMinId(
            this IMongoModelBuilder builder,
            Action<AbpMongoModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new MinIdMongoModelBuilderConfigurationOptions(
                MinIdDbProperties.DbTablePrefix
            );

            optionsAction?.Invoke(options);
        }
    }
}