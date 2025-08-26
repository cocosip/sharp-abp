using System;
using GaussDB.EntityFrameworkCore.PostgreSQL.Infrastructure;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public static class AbpDbContextConfigurationContextGaussDBExtensions
    {
        public static DbContextOptionsBuilder UseGaussDB(
            [NotNull] this AbpDbContextConfigurationContext context,
            Action<GaussDBDbContextOptionsBuilder>? dmOptionsAction = null)
        {
            if (context.ExistingConnection != null)
            {
                return context.DbContextOptions.UseGaussDB(context.ExistingConnection, optionsBuilder =>
                {
                    optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    dmOptionsAction?.Invoke(optionsBuilder);
                });
            }
            else
            {
                return context.DbContextOptions.UseGaussDB(context.ConnectionString, optionsBuilder =>
                {
                    optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    dmOptionsAction?.Invoke(optionsBuilder);
                });
            }
        }
    }
}
