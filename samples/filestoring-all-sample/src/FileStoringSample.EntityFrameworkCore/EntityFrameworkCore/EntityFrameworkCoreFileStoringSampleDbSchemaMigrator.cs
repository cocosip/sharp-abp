using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FileStoringSample.Data;
using Volo.Abp.DependencyInjection;

namespace FileStoringSample.EntityFrameworkCore
{
    public class EntityFrameworkCoreFileStoringSampleDbSchemaMigrator
        : IFileStoringSampleDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreFileStoringSampleDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the FileStoringSampleDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<FileStoringSampleDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}
