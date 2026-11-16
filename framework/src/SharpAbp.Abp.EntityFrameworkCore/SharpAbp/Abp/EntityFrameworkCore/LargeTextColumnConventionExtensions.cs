using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public static class LargeTextColumnConventionExtensions
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo?> LazyServiceProviderProperties = new();

        /// <summary>
        /// Registers <see cref="LargeTextColumnConvention"/> for the given DbContext. It is called
        /// automatically for every AbpDbContext by <see cref="SharpAbpEntityFrameworkCoreModule"/>.
        /// Call it manually when the host overrides AbpDbContextOptions.ConfigureDefaultConvention, or
        /// from ConfigureConventions when the DbContext is created without ABP property injection
        /// (for example a hand written IDesignTimeDbContextFactory), in which case the default options
        /// are used.
        /// </summary>
        public static ModelConfigurationBuilder AddSharpAbpLargeTextColumnConvention(
            this ModelConfigurationBuilder configurationBuilder,
            DbContext dbContext)
        {
            var providerName = dbContext.Database.ProviderName;
            if (string.IsNullOrWhiteSpace(providerName))
            {
                return configurationBuilder;
            }

            // AbpDbContext exposes its services through LazyServiceProvider, while the convention
            // factory below only gets the EF Core service provider, which cannot resolve them.
            var lazyServiceProvider = GetLazyServiceProvider(dbContext);

            IOptions<AbpLargeTextColumnOptions> options;
            IEfCoreDatabaseProviderAccessor databaseProviderAccessor;
            ILogger<LargeTextColumnConvention> logger;

            if (lazyServiceProvider != null)
            {
                options = lazyServiceProvider.LazyGetRequiredService<IOptions<AbpLargeTextColumnOptions>>();
                databaseProviderAccessor = lazyServiceProvider.LazyGetRequiredService<IEfCoreDatabaseProviderAccessor>();
                logger = lazyServiceProvider.LazyGetRequiredService<ILogger<LargeTextColumnConvention>>();
            }
            else
            {
                // DbContext without ABP property injection (design time factories, manual creation).
                // The convention is registered from ConfigureConventions by the DbContext itself.
                options = Microsoft.Extensions.Options.Options.Create(new AbpLargeTextColumnOptions());
                databaseProviderAccessor = new EfCoreDatabaseProviderAccessor(
                    NullLogger<EfCoreDatabaseProviderAccessor>.Instance);
                logger = NullLogger<LargeTextColumnConvention>.Instance;
            }

            configurationBuilder.Conventions.Add(_ => new LargeTextColumnConvention(
                providerName,
                options,
                databaseProviderAccessor,
                logger));

            return configurationBuilder;
        }

        private static IAbpLazyServiceProvider? GetLazyServiceProvider(DbContext dbContext)
        {
            var property = LazyServiceProviderProperties.GetOrAdd(
                dbContext.GetType(),
                type => type.GetProperty("LazyServiceProvider", BindingFlags.Public | BindingFlags.Instance));

            return property?.GetValue(dbContext) as IAbpLazyServiceProvider;
        }
    }
}
