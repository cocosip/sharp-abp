using SharpAbp.Abp.Data;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public interface IEfCoreDatabaseProviderAccessor
    {
        /// <summary>
        /// Get databaseProvider or null
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        DatabaseProvider? GetDatabaseProviderOrNull(string providerName);
    }
}
