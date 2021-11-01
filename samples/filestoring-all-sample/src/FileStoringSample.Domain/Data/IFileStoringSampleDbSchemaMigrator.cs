using System.Threading.Tasks;

namespace FileStoringSample.Data
{
    public interface IFileStoringSampleDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
