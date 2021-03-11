using Microsoft.EntityFrameworkCore;
using System;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SharpAbp.Abp.MapTenancyManagement.EntityFrameworkCore
{
    public static class MapTenancyManagementDbContextModelCreatingExtensions
    {
        public static void ConfigureMapTenancyManagement(
            this ModelBuilder builder,
            Action<MapTenancyManagementModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new MapTenancyManagementModelBuilderConfigurationOptions(
                MapTenancyManagementDbProperties.DbTablePrefix,
                MapTenancyManagementDbProperties.DbSchema
            );

            optionsAction?.Invoke(options);

            builder.Entity<MapTenant>(b =>
            {
                b.ToTable(options.TablePrefix + "MapTenants", options.Schema);

                b.ConfigureByConvention();

                b.Property(p => p.Code).IsRequired().HasMaxLength(MapTenantConsts.MaxCodeLength);

                b.Property(p => p.MapCode).HasMaxLength(MapTenantConsts.MaxMapCodeLength);

                b.HasIndex(p => new { p.MapCode }).IsUnique();
            });
        }
    }
}