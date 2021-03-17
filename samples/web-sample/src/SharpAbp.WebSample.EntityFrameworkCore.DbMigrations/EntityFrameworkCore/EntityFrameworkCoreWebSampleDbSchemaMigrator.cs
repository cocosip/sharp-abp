using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.WebSample.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.WebSample.EntityFrameworkCore
{
    public class EntityFrameworkCoreWebSampleDbSchemaMigrator
        : IWebSampleDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreWebSampleDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the WebSampleMigrationsDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<WebSampleMigrationsDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}