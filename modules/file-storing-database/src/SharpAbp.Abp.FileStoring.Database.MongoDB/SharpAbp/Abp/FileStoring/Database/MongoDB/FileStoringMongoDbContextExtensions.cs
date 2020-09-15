using System;
using Volo.Abp;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.FileStoring.Database.MongoDB
{
    public static class FileStoringMongoDbContextExtensions
    {
        public static void ConfigureFileStoring(
            this IMongoModelBuilder builder,
            Action<AbpMongoModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new FileStoringMongoModelBuilderConfigurationOptions(
                FileStoringDatabaseDbProperties.DbTablePrefix
            );

            optionsAction?.Invoke(options);

            builder.Entity<DatabaseFileContainer>(b =>
            {
                b.CollectionName = options.CollectionPrefix + "FileContainers";
            });

            builder.Entity<DatabaseFile>(b =>
            {
                b.CollectionName = options.CollectionPrefix + "Files";
            });
        }
    }
}
