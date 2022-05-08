using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MinIdApp.Data
{
    /* This is used if database provider does't define
     * IMinIdAppDbSchemaMigrator implementation.
     */
    public class NullMinIdAppDbSchemaMigrator : IMinIdAppDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}