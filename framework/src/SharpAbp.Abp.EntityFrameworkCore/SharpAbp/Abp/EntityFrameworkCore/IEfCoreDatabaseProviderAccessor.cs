using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public interface IEfCoreDatabaseProviderAccessor
    {
        /// <summary>
        /// Get databaseProvider or null
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        EfCoreDatabaseProvider? GetDatabaseProviderOrNull(string providerName);
    }
}
