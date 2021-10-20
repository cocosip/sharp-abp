using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IdentitySample.Data;
using Volo.Abp.DependencyInjection;

namespace IdentitySample.EntityFrameworkCore
{
    public class EntityFrameworkCoreIdentitySampleDbSchemaMigrator
        : IIdentitySampleDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreIdentitySampleDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the IdentitySampleDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<IdentitySampleDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}
