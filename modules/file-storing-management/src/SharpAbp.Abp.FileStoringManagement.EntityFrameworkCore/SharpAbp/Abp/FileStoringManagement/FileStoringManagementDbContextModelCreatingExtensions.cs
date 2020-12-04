using Microsoft.EntityFrameworkCore;
using System;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SharpAbp.Abp.FileStoringManagement
{
    public static class FileStoringManagementDbContextModelCreatingExtensions
    {
        public static void ConfigureFileStoringManagement(
          this ModelBuilder builder,
          Action<FileStoringManagementModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new FileStoringManagementModelBuilderConfigurationOptions(
                FileStoringManagementDbProperties.DbTablePrefix,
                FileStoringManagementDbProperties.DbSchema
            );

            optionsAction?.Invoke(options);

            builder.Entity<FileStoringContainer>(b =>
            {
                b.ToTable(options.TablePrefix + "FileStoringContainers", options.Schema);
                
                b.ConfigureByConvention();

                b.Property(p => p.Title).IsRequired().HasMaxLength(FileStoringContainerConsts.MaxTitleLength);
                
                b.Property(p => p.Name).IsRequired().HasMaxLength(FileStoringContainerConsts.MaxNameLength);
                
                b.Property(p => p.Provider).IsRequired().HasMaxLength(FileStoringContainerConsts.MaxProviderLength);
                
                b.Property(p => p.HttpSupport).IsRequired();
                
                b.Property(p => p.IsMultiTenant).IsRequired();
                
                b.HasMany(x => x.Items).WithOne().HasForeignKey(p => p.ContainerId).IsRequired();
                
                b.HasIndex(x => new { x.TenantId, x.Name });
                //b.HasIndex(x => x.Name);

            });

            builder.Entity<FileStoringContainerItem>(b =>
            {
                b.ToTable(options.TablePrefix + "FileStoringContainerItems", options.Schema);

                b.ConfigureByConvention();

                b.Property(p => p.ContainerId).IsRequired(); //TODO: Foreign key!

                b.Property(p => p.Name).IsRequired().HasMaxLength(FileStoringContainerItemConsts.MaxNameLength);

                b.Property(p => p.Value).IsRequired().HasMaxLength(FileStoringContainerItemConsts.MaxValueLength);

                b.HasIndex(x => new { x.ContainerId, x.Name });
            });
        }
    }
}
