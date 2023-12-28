using Microsoft.EntityFrameworkCore;
using System;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SharpAbp.MinId.EntityFrameworkCore
{
    public static class MinIdDbContextModelCreatingExtensions
    {
        public static void ConfigureMinId(
            this ModelBuilder builder,
            Action<MinIdModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new MinIdModelBuilderConfigurationOptions(
                MinIdDbProperties.DbTablePrefix,
                MinIdDbProperties.DbSchema
            );

            optionsAction?.Invoke(options);

            #region MinIdInfo
            //MinIdInfo
            builder.Entity<MinIdInfo>(b =>
            {
                b.ToTable(options.TablePrefix + "MinIdInfos", options.Schema);

                b.ConfigureByConvention();

                b.Property(p => p.BizType).IsRequired().HasMaxLength(MinIdInfoConsts.MaxBizTypeLength);

                b.Property(p => p.MaxId).IsRequired();

                b.Property(p => p.Step).IsRequired();

                b.Property(p => p.Delta).IsRequired();

                b.Property(p => p.Remainder).IsRequired();

                b.HasIndex(x => x.BizType).IsUnique();

                b.ApplyObjectExtensionMappings();
            });
            #endregion

            #region MinIdToken
            //MinIdToken
            builder.Entity<MinIdToken>(b =>
            {
                b.ToTable(options.TablePrefix + "MinIdTokens", options.Schema);

                b.ConfigureByConvention();

                b.Property(p => p.BizType).IsRequired().HasMaxLength(MinIdTokenConsts.MaxBizTypeLength);

                b.Property(p => p.Token).IsRequired().HasMaxLength(MinIdTokenConsts.MaxTokenLength);

                b.Property(p => p.Remark).HasMaxLength(MinIdTokenConsts.MaxRemarkLength);

                b.HasIndex(x => new { x.BizType, x.Token }).IsUnique();

                b.ApplyObjectExtensionMappings();
            });
            #endregion

            builder.TryConfigureObjectExtensions<MinIdDbContext>();
        }
    }
}