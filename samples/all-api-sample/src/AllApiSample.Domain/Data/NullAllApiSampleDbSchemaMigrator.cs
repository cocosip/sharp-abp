using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AllApiSample.Data;

/* This is used if database provider does't define
 * IAllApiSampleDbSchemaMigrator implementation.
 */
public class NullAllApiSampleDbSchemaMigrator : IAllApiSampleDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
