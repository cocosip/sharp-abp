using Microsoft.EntityFrameworkCore;
using System;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SharpAbp.Abp.FileStoring.Database
{
    public static class FileStoringDbContextModelCreatingExtensions
    {
        public static void ConfigureFileStoring(
            this ModelBuilder builder,
            Action<FileStoringModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new FileStoringModelBuilderConfigurationOptions(
                FileStoringDatabaseDbProperties.DbTablePrefix,
                FileStoringDatabaseDbProperties.DbSchema
            );

            optionsAction?.Invoke(options);

            builder.Entity<DatabaseFileContainer>(b =>
            {
                b.ToTable(options.TablePrefix + "FileContainers", options.Schema);

                b.ConfigureByConvention();

                b.Property(p => p.Name).IsRequired().HasMaxLength(DatabaseContainerConsts.MaxNameLength);

                b.Property(p => p.HttpAccess).IsRequired();

                b.Property(p => p.IncludeContainer).IsRequired();

                b.Property(p => p.HttpServer).HasMaxLength(DatabaseContainerConsts.MaxHttpServerLength);

                b.HasMany<DatabaseFile>().WithOne().HasForeignKey(p => p.ContainerId);

                b.HasIndex(p => new { p.TenantId, p.Name });
            });

            builder.Entity<DatabaseFile>(b =>
            {
                b.ToTable(options.TablePrefix + "Files", options.Schema);

                b.ConfigureByConvention();

                b.Property(p => p.ContainerId).IsRequired(); //TODO: Foreign key!
                b.Property(p => p.Name).IsRequired().HasMaxLength(DatabaseFileConsts.MaxNameLength);
                b.Property(p => p.Content).HasMaxLength(DatabaseFileConsts.MaxContentLength);

                b.HasOne<DatabaseFileContainer>().WithMany().HasForeignKey(p => p.ContainerId);

                b.HasIndex(p => new { p.TenantId, p.ContainerId, p.Name });
            });
        }
    }
}
