using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IdentityModelSample.Data;
using Volo.Abp.DependencyInjection;

namespace IdentityModelSample.EntityFrameworkCore
{
    public class EntityFrameworkCoreIdentityModelSampleDbSchemaMigrator
        : IIdentityModelSampleDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreIdentityModelSampleDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the IdentityModelSampleDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<IdentityModelSampleDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}
