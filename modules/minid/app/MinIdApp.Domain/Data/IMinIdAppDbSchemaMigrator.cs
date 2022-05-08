using System.Threading.Tasks;

namespace MinIdApp.Data
{
    public interface IMinIdAppDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
