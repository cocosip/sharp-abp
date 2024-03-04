using Microsoft.EntityFrameworkCore;
using SharpAbp.Abp.CryptoVault;
using System;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SharpAbp.Abp.TransformSecurityManagement.EntityFrameworkCore
{
    public static class TransformSecurityManagementDbContextModelCreatingExtensions
    {
        public static void ConfigureTransformSecurityManagement(
            this ModelBuilder builder,
            Action<AbpTransformSecurityManagementModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new AbpTransformSecurityManagementModelBuilderConfigurationOptions(
                AbpCryptoVaultDbProperties.DbTablePrefix,
                AbpCryptoVaultDbProperties.DbSchema
            );

            optionsAction?.Invoke(options);

            builder.Entity<SecurityCredentialInfo>(b =>
            {
                b.ToTable(options.TablePrefix + "SecurityCredentialInfos", options.Schema);

                b.ConfigureByConvention();

                b.Property(p => p.Identifier).IsRequired().HasMaxLength(SecurityCredentialInfoConsts.MaxIdentifierLength);

                b.Property(p => p.CredsId).IsRequired();

                b.Property(p => p.KeyType).IsRequired().HasMaxLength(SecurityCredentialInfoConsts.MaxKeyTypeLength);

                b.Property(p => p.BizType).IsRequired().HasMaxLength(SecurityCredentialInfoConsts.MaxBizTypeLength);

                b.Property(p => p.Description).HasMaxLength(SecurityCredentialInfoConsts.MaxDescriptionLength);

                b.HasIndex(p => new { p.Identifier }).IsUnique();

                b.ApplyObjectExtensionMappings();
            });
 
            builder.TryConfigureObjectExtensions<AbpTransformSecurityManagementDbContext>();
        }
    }
}
