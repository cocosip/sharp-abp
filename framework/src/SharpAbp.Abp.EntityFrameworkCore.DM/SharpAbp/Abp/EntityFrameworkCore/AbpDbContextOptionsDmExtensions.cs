using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public static class AbpDbContextOptionsDmExtensions
    {
        public static void UseDm(
            [NotNull] this AbpDbContextOptions options,
            Action<DmDbContextOptionsBuilder> dmOptionsAction = null)
        {
            options.Configure(context =>
            {
                context.UseDm(dmOptionsAction);
            });
        }

        public static void UseDm<TDbContext>(
            [NotNull] this AbpDbContextOptions options,
            Action<DmDbContextOptionsBuilder> dmOptionsAction = null)
            where TDbContext : AbpDbContext<TDbContext>
        {
            options.Configure<TDbContext>(context =>
            {
                context.UseDm(dmOptionsAction);
            });
        }
    }
}
