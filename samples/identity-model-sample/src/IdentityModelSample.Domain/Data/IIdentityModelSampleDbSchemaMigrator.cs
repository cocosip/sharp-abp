using System.Threading.Tasks;

namespace IdentityModelSample.Data
{
    public interface IIdentityModelSampleDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
