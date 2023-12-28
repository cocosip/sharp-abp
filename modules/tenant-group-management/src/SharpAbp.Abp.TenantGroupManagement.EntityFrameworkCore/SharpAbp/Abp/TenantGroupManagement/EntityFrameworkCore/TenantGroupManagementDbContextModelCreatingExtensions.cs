using Microsoft.EntityFrameworkCore;
using System;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SharpAbp.Abp.TenantGroupManagement.EntityFrameworkCore
{
    public static class TenantGroupManagementDbContextModelCreatingExtensions
    {
        public static void ConfigureTenantGroupManagement(
            this ModelBuilder builder,
            Action<TenantGroupManagementModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new TenantGroupManagementModelBuilderConfigurationOptions(
                TenantGroupManagementDbProperties.DbTablePrefix,
                TenantGroupManagementDbProperties.DbSchema
            );

            optionsAction?.Invoke(options);

            builder.Entity<TenantGroup>(b =>
            {
                b.ToTable(options.TablePrefix + "TenantGroups", options.Schema);

                b.ConfigureByConvention();

                b.Property(p => p.Name).IsRequired().HasMaxLength(TenantGroupConsts.MaxNameLength);

                b.HasIndex(p => new { p.Name }).IsUnique();

                b.HasMany(p => p.ConnectionStrings).WithOne().HasForeignKey(uc => uc.TenantGroupId).IsRequired();

                b.HasMany(p => p.Tenants).WithOne().HasForeignKey(uc => uc.TenantGroupId).IsRequired();

                b.ApplyObjectExtensionMappings();
            });

            builder.Entity<TenantGroupTenant>(b =>
            {
                b.ToTable(options.TablePrefix + "TenantGroupTenants", options.Schema);

                b.ConfigureByConvention();

                b.Property(p => p.TenantGroupId).IsRequired();

                b.Property(p => p.TenantId).IsRequired();

                b.HasIndex(p => new { p.TenantGroupId, p.TenantId }).IsUnique();

                b.ApplyObjectExtensionMappings();
            });

            builder.Entity<TenantGroupConnectionString>(b =>
            {
                b.ToTable(options.TablePrefix + "TenantGroupConnectionStrings", options.Schema);

                b.ConfigureByConvention();

                b.Property(cs => cs.Name).IsRequired().HasMaxLength(TenantGroupConnectionStringConsts.MaxNameLength);

                b.Property(cs => cs.Value).IsRequired().HasMaxLength(TenantGroupConnectionStringConsts.MaxValueLength);

                b.HasIndex(p => new { p.TenantGroupId, p.Name }).IsUnique();

                b.ApplyObjectExtensionMappings();
            });


        }
    }
}
