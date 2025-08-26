using SharpAbp.Abp.Data;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public interface IEfCoreDatabaseProviderAccessor
    {
        /// <summary>
        /// Gets the database provider enum value based on the provider name, or null if not found.
        /// </summary>
        /// <param name="providerName">The name of the database provider.</param>
        /// <returns>The corresponding DatabaseProvider enum value, or null if the provider name is not recognized.</returns>
        DatabaseProvider? GetDatabaseProviderOrNull(string providerName);
    }
}
