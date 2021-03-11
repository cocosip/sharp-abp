using System;
using Volo.Abp;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.MapTenancyManagement.MongoDB
{
    public static class MapTenancyManagementMongoDbContextExtensions
    {
        public static void ConfigureMapTenancyManagement(
            this IMongoModelBuilder builder,
            Action<AbpMongoModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new MapTenancyManagementMongoModelBuilderConfigurationOptions(
                MapTenancyManagementDbProperties.DbTablePrefix
            );

            optionsAction?.Invoke(options);

            builder.Entity<MapTenant>(b =>
            {
                b.CollectionName = options.CollectionPrefix + "MapTenants";
            });
        }
    }
}