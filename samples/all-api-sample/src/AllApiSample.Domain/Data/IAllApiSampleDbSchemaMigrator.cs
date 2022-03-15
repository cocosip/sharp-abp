using System.Threading.Tasks;

namespace AllApiSample.Data;

public interface IAllApiSampleDbSchemaMigrator
{
    Task MigrateAsync();
}
