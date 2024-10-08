﻿using Microsoft.EntityFrameworkCore;
using System;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SharpAbp.Abp.CryptoVault.EntityFrameworkCore
{
    public static class CryptoVaultDbContextModelCreatingExtensions
    {
        public static void ConfigureCryptoVault(
            this ModelBuilder builder,
            Action<AbpCryptoVaultModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new AbpCryptoVaultModelBuilderConfigurationOptions(
                AbpCryptoVaultDbProperties.DbTablePrefix,
                AbpCryptoVaultDbProperties.DbSchema
            );

            optionsAction?.Invoke(options);

            builder.Entity<RSACreds>(b =>
            {
                b.ToTable(options.TablePrefix + "RSACreds", options.Schema);

                b.ConfigureByConvention();

                b.Property(p => p.Identifier).IsRequired().HasMaxLength(RSACredsConsts.MaxIdentifierLength);

                b.Property(p => p.SourceType).IsRequired();

                b.Property(p => p.Size).IsRequired();

                b.Property(p => p.PublicKey).IsRequired().HasMaxLength(RSACredsConsts.MaxPublicKeyLength);

                b.Property(p => p.PrivateKey).IsRequired().HasMaxLength(RSACredsConsts.MaxPrivateKeyLength);

                b.Property(p => p.PassPhrase).IsRequired().HasMaxLength(RSACredsConsts.MaxPassPhraseLength);

                b.Property(p => p.Salt).IsRequired().HasMaxLength(RSACredsConsts.MaxSaltLength);

                b.Property(p => p.Description).HasMaxLength(RSACredsConsts.MaxDescriptionLength);

                b.HasIndex(p => new { p.Identifier }).IsUnique();

                b.ApplyObjectExtensionMappings();
            });

            builder.Entity<SM2Creds>(b =>
            {
                b.ToTable(options.TablePrefix + "SM2Creds", options.Schema);

                b.ConfigureByConvention();

                b.Property(p => p.Identifier).IsRequired().HasMaxLength(SM2CredsConsts.MaxIdentifierLength);

                b.Property(p => p.SourceType).IsRequired();

                b.Property(p => p.Curve).IsRequired().HasMaxLength(SM2CredsConsts.MaxCurveLength);

                b.Property(p => p.PassPhrase).IsRequired().HasMaxLength(SM2CredsConsts.MaxPassPhraseLength);

                b.Property(p => p.Salt).IsRequired().HasMaxLength(SM2CredsConsts.MaxSaltLength);

                b.Property(p => p.PublicKey).IsRequired().HasMaxLength(SM2CredsConsts.MaxPublicKeyLength);

                b.Property(p => p.PrivateKey).IsRequired().HasMaxLength(SM2CredsConsts.MaxPrivateKeyLength);

                b.Property(p => p.Description).HasMaxLength(SM2CredsConsts.MaxDescriptionLength);

                b.HasIndex(p => new { p.Identifier }).IsUnique();

                b.ApplyObjectExtensionMappings();
            });

            builder.TryConfigureObjectExtensions<AbpCryptoVaultDbContext>();
        }
    }
}
