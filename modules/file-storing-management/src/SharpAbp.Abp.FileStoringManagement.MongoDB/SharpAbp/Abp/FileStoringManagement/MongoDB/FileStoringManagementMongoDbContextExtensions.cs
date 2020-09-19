using System;
using Volo.Abp;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.FileStoringManagement.MongoDB
{
    public static class FileStoringManagementMongoDbContextExtensions
    {
        public static void ConfigureFileStoringManagement(
            this IMongoModelBuilder builder,
            Action<AbpMongoModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new FileStoringManagementMongoModelBuilderConfigurationOptions(
                FileStoringManagementDbProperties.DbTablePrefix
            );

            optionsAction?.Invoke(options);

            builder.Entity<FileStoringContainer>(b =>
            {
                b.CollectionName = options.CollectionPrefix + "FileStoringContainers";
            });

            builder.Entity<FileStoringContainerItem>(b =>
            {
                b.CollectionName = options.CollectionPrefix + "FileStoringContainerItems";
            });
        }
    }
}
