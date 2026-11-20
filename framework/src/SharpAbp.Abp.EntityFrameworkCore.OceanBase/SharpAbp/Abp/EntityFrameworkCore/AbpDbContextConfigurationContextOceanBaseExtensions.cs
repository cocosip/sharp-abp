using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OceanBase.EntityFrameworkCore;
using OceanBase.EntityFrameworkCore.Infrastructure;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using SharpAbp.Abp.EntityFrameworkCore.OceanBase;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public static class AbpDbContextConfigurationContextOceanBaseExtensions
    {
        public static DbContextOptionsBuilder UseOceanBase(
            this AbpDbContextConfigurationContext context,
            OceanBaseCompatibilityMode compatibilityMode,
            Action<OceanBaseDbContextOptionsBuilder>? oceanBaseOptionsAction = null)
        {
            Check.NotNull(context, nameof(context));
            AbpDbContextOptionsOceanBaseExtensions.ValidateMode(compatibilityMode);

            void Configure(OceanBaseDbContextOptionsBuilder builder)
            {
                builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                oceanBaseOptionsAction?.Invoke(builder);
            }

            var builder = context.ExistingConnection != null
                ? context.DbContextOptions.UseOceanBase(context.ExistingConnection, compatibilityMode, Configure)
                : context.DbContextOptions.UseOceanBase(context.ConnectionString, compatibilityMode, Configure);
            ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(
                new OceanBaseAbpOptionsExtension(compatibilityMode));
            return builder;
        }
    }
}
