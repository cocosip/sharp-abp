using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace IdentityModelSample.Data
{
    /* This is used if database provider does't define
     * IIdentityModelSampleDbSchemaMigrator implementation.
     */
    public class NullIdentityModelSampleDbSchemaMigrator : IIdentityModelSampleDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}