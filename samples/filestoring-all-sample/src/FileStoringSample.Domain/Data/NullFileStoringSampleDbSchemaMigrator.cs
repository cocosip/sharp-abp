using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace FileStoringSample.Data
{
    /* This is used if database provider does't define
     * IFileStoringSampleDbSchemaMigrator implementation.
     */
    public class NullFileStoringSampleDbSchemaMigrator : IFileStoringSampleDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}