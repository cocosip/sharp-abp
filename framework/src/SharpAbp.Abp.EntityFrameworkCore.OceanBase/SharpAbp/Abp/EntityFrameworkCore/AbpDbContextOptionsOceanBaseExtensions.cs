using System;
using OceanBase.EntityFrameworkCore;
using OceanBase.EntityFrameworkCore.Infrastructure;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public static class AbpDbContextOptionsOceanBaseExtensions
    {
        public static void UseOceanBase(
            this AbpDbContextOptions options,
            OceanBaseCompatibilityMode compatibilityMode,
            Action<OceanBaseDbContextOptionsBuilder>? oceanBaseOptionsAction = null)
        {
            Check.NotNull(options, nameof(options));
            ValidateMode(compatibilityMode);
            options.Configure(context => context.UseOceanBase(compatibilityMode, oceanBaseOptionsAction));
        }

        public static void UseOceanBase<TDbContext>(
            this AbpDbContextOptions options,
            OceanBaseCompatibilityMode compatibilityMode,
            Action<OceanBaseDbContextOptionsBuilder>? oceanBaseOptionsAction = null)
            where TDbContext : AbpDbContext<TDbContext>
        {
            Check.NotNull(options, nameof(options));
            ValidateMode(compatibilityMode);
            options.Configure<TDbContext>(context => context.UseOceanBase(compatibilityMode, oceanBaseOptionsAction));
        }

        internal static void ValidateMode(OceanBaseCompatibilityMode compatibilityMode)
        {
            if (compatibilityMode is not (OceanBaseCompatibilityMode.MySql or OceanBaseCompatibilityMode.Oracle))
            {
                throw new ArgumentOutOfRangeException(nameof(compatibilityMode));
            }
        }
    }
}
