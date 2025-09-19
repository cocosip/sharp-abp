using System;
using Microsoft.Extensions.Logging;
using SharpAbp.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    /// <summary>
    /// Provides access to database providers for Entity Framework Core.
    /// Maps provider names to their corresponding DatabaseProvider enum values.
    /// </summary>
    public class EfCoreDatabaseProviderAccessor : IEfCoreDatabaseProviderAccessor, ITransientDependency
    {
        protected ILogger Logger { get; }

        public EfCoreDatabaseProviderAccessor(ILogger<EfCoreDatabaseProviderAccessor> logger)
        {
            Logger = logger;
        }
        
        /// <summary>
        /// Gets the database provider enum value based on the provider name, or null if not found.
        /// </summary>
        /// <param name="providerName">The name of the database provider.</param>
        /// <returns>The corresponding DatabaseProvider enum value, or null if the provider name is not recognized.</returns>
        public virtual DatabaseProvider? GetDatabaseProviderOrNull(string providerName)
        {
            if (providerName.IsNullOrWhiteSpace())
            {
                Logger.LogDebug("Database provider name is null or whitespace, returning null");
                return null;
            }

            var result = providerName.ToUpperInvariant() switch
            {
                "SQLSERVER" => (DatabaseProvider?)DatabaseProvider.SqlServer,
                "POSTGRESQL" => (DatabaseProvider?)DatabaseProvider.PostgreSql,
                "MYSQL" => (DatabaseProvider?)DatabaseProvider.MySql,
                "ORACLE" or "DEVART.ORACLE" => (DatabaseProvider?)DatabaseProvider.Oracle,
                "SQLITE" => (DatabaseProvider?)DatabaseProvider.Sqlite,
                "INMEMORY" => (DatabaseProvider?)DatabaseProvider.InMemory,
                "FIREBIRD" => (DatabaseProvider?)DatabaseProvider.Firebird,
                "COSMOS" => (DatabaseProvider?)DatabaseProvider.Cosmos,
                "DM" => (DatabaseProvider?)DatabaseProvider.Dm,
                "OPENGAUSS" => (DatabaseProvider?)DatabaseProvider.OpenGauss,
                "GAUSSDB" => (DatabaseProvider?)DatabaseProvider.GaussDB,
                _ => null,
            };

            Logger.LogDebug("Resolved database provider: {DatabaseProvider} for provider name: {ProviderName}", result, providerName);

            return result;
        }
    }
}