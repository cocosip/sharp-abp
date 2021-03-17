using System.Threading.Tasks;

namespace SharpAbp.WebSample.Data
{
    public interface IWebSampleDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
