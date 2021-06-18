using System;
using Volo.Abp;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.DbConnectionsManagement.MongoDB
{
    public static class DbConnectionsManagementMongoDbContextExtensions
    {
        public static void ConfigureDbConnectionsManagement(
            this IMongoModelBuilder builder,
            Action<AbpMongoModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new DbConnectionsManagementMongoModelBuilderConfigurationOptions(
                DbConnectionsManagementDbProperties.DbTablePrefix
            );

            optionsAction?.Invoke(options);

            builder.Entity<DatabaseConnectionInfo>(b =>
            {
                b.CollectionName = options.CollectionPrefix + "DatabaseConnectionInfos";
            });
        }
    }
}
