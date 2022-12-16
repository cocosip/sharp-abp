using System.Threading.Tasks;

namespace SharpSample.Data;

public interface ISharpSampleDbSchemaMigrator
{
    Task MigrateAsync();
}
