using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpSample.Data;

/* This is used if database provider does't define
 * ISharpSampleDbSchemaMigrator implementation.
 */
public class NullSharpSampleDbSchemaMigrator : ISharpSampleDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
