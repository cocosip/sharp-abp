using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace IdentitySample.Data
{
    /* This is used if database provider does't define
     * IIdentitySampleDbSchemaMigrator implementation.
     */
    public class NullIdentitySampleDbSchemaMigrator : IIdentitySampleDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}