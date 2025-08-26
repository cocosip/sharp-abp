using System;
using GaussDB.EntityFrameworkCore.PostgreSQL.Infrastructure;
using JetBrains.Annotations;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public static class AbpDbContextOptionsGaussDBExtensions
    {
        public static void UseGaussDB(
            [NotNull] this AbpDbContextOptions options,
            Action<GaussDBDbContextOptionsBuilder>? gaussDBOptionsAction = null)
        {
            options.Configure(context =>
            {
                context.UseGaussDB(gaussDBOptionsAction);
            });
        }

        public static void UseGaussDB<TDbContext>(
            [NotNull] this AbpDbContextOptions options,
            Action<GaussDBDbContextOptionsBuilder>? dmOptionsAction = null)
            where TDbContext : AbpDbContext<TDbContext>
        {
            options.Configure<TDbContext>(context =>
            {
                context.UseGaussDB(dmOptionsAction);
            });
        }
    }
}
