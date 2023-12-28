using Microsoft.EntityFrameworkCore;
using System;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SharpAbp.Abp.DbConnectionsManagement.EntityFrameworkCore
{
    public static class DbConnectionsManagementDbContextModelCreatingExtensions
    {
        public static void ConfigureDbConnectionsManagement(
            this ModelBuilder builder,
            Action<DbConnectionsManagementModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new DbConnectionsManagementModelBuilderConfigurationOptions(
                DbConnectionsManagementDbProperties.DbTablePrefix,
                DbConnectionsManagementDbProperties.DbSchema
            );

            optionsAction?.Invoke(options);

            builder.Entity<DatabaseConnectionInfo>(b =>
            {
                b.ToTable(options.TablePrefix + "DatabaseConnectionInfos", options.Schema);

                b.ConfigureByConvention();

                b.Property(p => p.Name).IsRequired().HasMaxLength(DatabaseConnectionInfoConsts.MaxNameLength);

                b.Property(p => p.DatabaseProvider).IsRequired().HasMaxLength(DatabaseConnectionInfoConsts.MaxDatabaseProviderLength);

                b.Property(p => p.ConnectionString).HasMaxLength(DatabaseConnectionInfoConsts.MaxConnectionStringLength);

                b.HasIndex(p => new { p.Name }).IsUnique();

                b.ApplyObjectExtensionMappings();
            });
        }
    }
}
