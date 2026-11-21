using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Volo.Abp;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.TenantGroupManagement.MongoDB
{
    public static class TenantGroupManagementMongoDbContextExtensions
    {
        /// <summary>
        /// Creates the unique membership index on the tenant group collection during a deployment migration.
        /// </summary>
        public static Task<string> CreateUniqueTenantMembershipIndexAsync(
            this IMongoCollection<TenantGroup> tenantGroups,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(tenantGroups, nameof(tenantGroups));

            var field = $"{nameof(TenantGroup.Tenants)}.{nameof(TenantGroupTenant.TenantId)}";
            var index = new CreateIndexModel<TenantGroup>(
                Builders<TenantGroup>.IndexKeys.Ascending(field),
                new CreateIndexOptions<TenantGroup>
                {
                    Name = "UX_TenantGroups_TenantId",
                    Unique = true,
                    PartialFilterExpression = Builders<TenantGroup>.Filter.Exists(field)
                });
            return tenantGroups.Indexes.CreateOneAsync(index, cancellationToken: cancellationToken);
        }

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
