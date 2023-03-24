using System;
using Volo.Abp;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.TenantGroupManagement.MongoDB
{
    public static class TenantGroupManagementMongoDbContextExtensions
    {
        public static void ConfigureTenantGroupManagement(
            this IMongoModelBuilder builder,
            Action<AbpMongoModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new TenantGroupManagementMongoModelBuilderConfigurationOptions(
                TenantGroupManagementDbProperties.DbTablePrefix
            );

            optionsAction?.Invoke(options);

            builder.Entity<TenantGroup>(b =>
            {
                b.CollectionName = options.CollectionPrefix + "TenantGroups";
            });

            builder.Entity<TenantGroupTenant>(b =>
            {
                b.CollectionName = options.CollectionPrefix + "TenantGroupTenants";
            });

            builder.Entity<TenantGroupConnectionString>(b =>
            {
                b.CollectionName = options.CollectionPrefix + "TenantGroupConnectionStrings";
            });
        }
    }
}
