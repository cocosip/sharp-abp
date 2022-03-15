using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AllApiSample.Data;
using Volo.Abp.DependencyInjection;

namespace AllApiSample.EntityFrameworkCore;

public class EntityFrameworkCoreAllApiSampleDbSchemaMigrator
    : IAllApiSampleDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreAllApiSampleDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the AllApiSampleDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<AllApiSampleDbContext>()
            .Database
            .MigrateAsync();
    }
}
