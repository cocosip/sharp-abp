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
        /// <summary>
        /// Logger instance for logging operations
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreDatabaseProviderAccessor"/> class.
        /// </summary>
        /// <param name="logger">The logger instance to use for logging</param>
        public EfCoreDatabaseProviderAccessor(ILogger<EfCoreDatabaseProviderAccessor> logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Gets the database provider enum value based on the custom provider name, or null if not found.
        /// This method handles custom provider names like "SQLSERVER", "POSTGRESQL", etc.
        /// </summary>
        /// <param name="providerName">The custom name of the database provider (e.g. "SQLSERVER", "MYSQL")</param>
        /// <returns>The corresponding DatabaseProvider enum value, or null if the provider name is not recognized</returns>
        public virtual DatabaseProvider? GetDatabaseProviderOrNull(string providerName)
        {
            if (providerName.IsNullOrWhiteSpace())
            {
                Logger.LogDebug("Custom database provider name is null or whitespace, returning null");
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

            Logger.LogDebug("Resolved database provider '{DatabaseProvider}' for custom provider name '{ProviderName}'", result, providerName);

            return result;
        }

        /// <summary>
        /// Gets the database provider enum value based on the EF Core provider name, or null if not found.
        /// This method handles full EF Core provider names like "Microsoft.EntityFrameworkCore.SqlServer".
        /// </summary>
        /// <param name="providerName">The full EF Core provider name (e.g. "Microsoft.EntityFrameworkCore.SqlServer")</param>
        /// <returns>The corresponding DatabaseProvider enum value, or null if the provider name is not recognized</returns>
        public virtual DatabaseProvider? GetDatabaseProviderOrNullByEfCoreName(string providerName)
        {
            if (providerName.IsNullOrWhiteSpace())
            {
                Logger.LogDebug("EF Core database provider name is null or whitespace, returning null");
                return null;
            }

            var result = providerName switch
            {
                "Microsoft.EntityFrameworkCore.SqlServer" => (DatabaseProvider?)DatabaseProvider.SqlServer,
                "Npgsql.EntityFrameworkCore.PostgreSQL" => (DatabaseProvider?)DatabaseProvider.PostgreSql,
                "Pomelo.EntityFrameworkCore.MySql" or "MySql.Data.EntityFrameworkCore" => (DatabaseProvider?)DatabaseProvider.MySql,
                "Oracle.EntityFrameworkCore" or "Devart.Data.Oracle.Entity.EFCore" => (DatabaseProvider?)DatabaseProvider.Oracle,
                "Microsoft.EntityFrameworkCore.Sqlite" => (DatabaseProvider?)DatabaseProvider.Sqlite,
                "Microsoft.EntityFrameworkCore.InMemory" => (DatabaseProvider?)DatabaseProvider.InMemory,
                "FirebirdSql.EntityFrameworkCore.Firebird" => (DatabaseProvider?)DatabaseProvider.Firebird,
                "Microsoft.EntityFrameworkCore.Cosmos" => (DatabaseProvider?)DatabaseProvider.Cosmos,
                "Dm.EntityFrameworkCore" => (DatabaseProvider?)DatabaseProvider.Dm,
                "OpenG.EntityFrameworkCore.OpenGauss" => (DatabaseProvider?)DatabaseProvider.OpenGauss,
                "EntityFrameworkCore.GaussDB" => (DatabaseProvider?)DatabaseProvider.GaussDB,
                _ => null
            };

            Logger.LogDebug("Resolved database provider '{DatabaseProvider}' for EF Core provider name '{ProviderName}'", result, providerName);

            return result;
        }

    }
}