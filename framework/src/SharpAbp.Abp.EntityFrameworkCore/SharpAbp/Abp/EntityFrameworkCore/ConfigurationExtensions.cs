using Microsoft.Extensions.Configuration;
using SharpAbp.Abp.Data;
using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    /// <summary>
    /// Extension methods for IConfiguration to work with Entity Framework Core options.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Gets the database provider from configuration.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="defaultValue">The default database provider if not specified in configuration.</param>
        /// <returns>The configured database provider or default value.</returns>
        public static DatabaseProvider GetDatabaseProvider(
            this IConfiguration configuration,
            DatabaseProvider defaultValue = DatabaseProvider.PostgreSql)
        {
            if (Enum.TryParse(configuration["EfCoreOptions:DatabaseProvider"], out DatabaseProvider databaseProvider))
            {
                return databaseProvider;
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets the EF Core properties from configuration.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>A dictionary of properties or null if not found.</returns>
        public static Dictionary<string, string>? GetProperties(this IConfiguration configuration)
        {
            var options = configuration.GetSection("EfCoreOptions").Get<SharpAbpEfCoreOptions>();
            return options?.Properties;
        }


        /// <summary>
        /// Gets the PostgreSQL version setting from configuration.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The PostgreSQL version.</returns>
        public static string GetPostgreSqlVersion(this IConfiguration configuration)
        {
            return configuration.GetProperties().GetPropertyValue(EfCoreConstants.PropertyNames.PostgreSqlVersion, "");
        }

        /// <summary>
        /// Gets the Oracle SQL compatibility setting from configuration.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The Oracle SQL compatibility version.</returns>
        public static string GetOracleSQLCompatibility(this IConfiguration configuration)
        {
            return configuration.GetProperties().GetPropertyValue(
                EfCoreConstants.PropertyNames.OracleSQLCompatibility,
                EfCoreConstants.DefaultValues.OracleSQLCompatibility,
                EfCoreConstants.ValidOracleSqlCompatibilityVersions);
        }

        /// <summary>
        /// Gets the Oracle allowed logon version client setting from configuration.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The Oracle allowed logon version client.</returns>
        public static string GetOracleAllowedLogonVersionClient(this IConfiguration configuration)
        {
            return configuration.GetProperties().GetPropertyValue(
                EfCoreConstants.PropertyNames.OracleAllowedLogonVersionClient,
                EfCoreConstants.DefaultValues.OracleAllowedLogonVersionClient,
                EfCoreConstants.ValidOracleLogonVersions);
        }

        /// <summary>
        /// Gets the MySQL version setting from configuration.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The MySQL version.</returns>
        public static string GetMySqlVersion(this IConfiguration configuration)
        {
            return configuration.GetProperties().GetPropertyValue(
                EfCoreConstants.PropertyNames.MySqlVersion, 
                EfCoreConstants.DefaultValues.MySqlVersion);
        }

        /// <summary>
        /// Gets the MySQL server type setting from configuration.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The MySQL server type.</returns>
        public static string GetMySqlServerType(this IConfiguration configuration)
        {
            return configuration.GetProperties().GetPropertyValue(
                EfCoreConstants.PropertyNames.MySqlServerType, 
                EfCoreConstants.DefaultValues.MySqlServerType);
        }

        /// <summary>
        /// Gets the default schema setting from configuration.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The default schema.</returns>
        public static string GetDefaultSchema(this IConfiguration configuration)
        {
            return configuration.GetProperties().GetPropertyValue(EfCoreConstants.PropertyNames.DefaultSchema, "");
        }


    }
}