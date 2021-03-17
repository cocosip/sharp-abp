using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.WebSample.Data
{
    /* This is used if database provider does't define
     * IWebSampleDbSchemaMigrator implementation.
     */
    public class NullWebSampleDbSchemaMigrator : IWebSampleDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}