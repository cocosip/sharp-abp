using SharpAbp.Abp.Data;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public interface IEfCoreDatabaseProviderAccessor
    {

        /// <summary>
        /// Gets the database provider enum value based on the custom provider name, or null if not found.
        /// This method handles custom provider names like "SQLSERVER", "POSTGRESQL", etc.
        /// </summary>
        /// <param name="providerName">The custom name of the database provider (e.g. "SQLSERVER", "MYSQL")</param>
        /// <returns>The corresponding DatabaseProvider enum value, or null if the provider name is not recognized</returns>
        DatabaseProvider? GetDatabaseProviderOrNull(string providerName);

        /// <summary>
        /// Gets the database provider enum value based on the EF Core provider name, or null if not found.
        /// This method handles full EF Core provider names like "Microsoft.EntityFrameworkCore.SqlServer".
        /// </summary>
        /// <param name="providerName">The full EF Core provider name (e.g. "Microsoft.EntityFrameworkCore.SqlServer")</param>
        /// <returns>The corresponding DatabaseProvider enum value, or null if the provider name is not recognized</returns>
        DatabaseProvider? GetDatabaseProviderOrNullByEfCoreName(string providerName);
    }
}
