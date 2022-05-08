using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MinIdApp.Data;
using Volo.Abp.DependencyInjection;

namespace MinIdApp.EntityFrameworkCore
{
    public class EntityFrameworkCoreMinIdAppDbSchemaMigrator
        : IMinIdAppDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreMinIdAppDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the MinIdAppMigrationsDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<MinIdAppMigrationsDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}