using System.Threading.Tasks;

namespace IdentitySample.Data
{
    public interface IIdentitySampleDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
