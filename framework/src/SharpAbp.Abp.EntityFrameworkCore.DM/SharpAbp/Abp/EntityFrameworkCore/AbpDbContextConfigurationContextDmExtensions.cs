using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public static class AbpDbContextConfigurationContextDmExtensions
    {
        public static DbContextOptionsBuilder UseDm(
            [NotNull] this AbpDbContextConfigurationContext context,
            Action<DmDbContextOptionsBuilder> dmOptionsAction = null)
        {
            if (context.ExistingConnection != null)
            {
                return context.DbContextOptions.UseDm(context.ExistingConnection, optionsBuilder =>
                {
                    //optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    dmOptionsAction?.Invoke(optionsBuilder);
                });
            }
            else
            {
                return context.DbContextOptions.UseDm(context.ConnectionString, optionsBuilder =>
                {
                    //optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    dmOptionsAction?.Invoke(optionsBuilder);
                });
            }
        }
    }
}
