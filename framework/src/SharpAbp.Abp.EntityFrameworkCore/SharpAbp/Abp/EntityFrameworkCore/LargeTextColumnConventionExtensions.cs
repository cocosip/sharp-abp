using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        /// automatically for every AbpDbContext by <see cref="SharpAbpEntityFrameworkCoreModule"/>;
        /// call it manually when the host overrides AbpDbContextOptions.ConfigureDefaultConvention.
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
            if (lazyServiceProvider == null)
            {
                return configurationBuilder;
            }

            var options = lazyServiceProvider.LazyGetRequiredService<IOptions<AbpLargeTextColumnOptions>>();
            var databaseProviderAccessor = lazyServiceProvider.LazyGetRequiredService<IEfCoreDatabaseProviderAccessor>();
            var logger = lazyServiceProvider.LazyGetRequiredService<ILogger<LargeTextColumnConvention>>();

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
