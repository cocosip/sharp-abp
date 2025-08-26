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
        /// Gets the Oracle SQL compatibility setting from configuration.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The Oracle SQL compatibility version.</returns>
        public static string GetOracleSQLCompatibility(this IConfiguration configuration)
        {
            return configuration.GetProperties().GetPropertyValue(
                "OracleSQLCompatibility",
                "DatabaseVersion19",
                ["DatabaseVersion19", "DatabaseVersion21", "DatabaseVersion23"]);
        }

        /// <summary>
        /// Gets the Oracle allowed logon version client setting from configuration.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The Oracle allowed logon version client.</returns>
        public static string GetOracleAllowedLogonVersionClient(this IConfiguration configuration)
        {
            return configuration.GetProperties().GetPropertyValue(
                "OracleAllowedLogonVersionClient",
                "DatabaseVersion19",
                ["Version8", "Version9", "Version10", "Version11", "Version12", "Version12a"]);
        }

        /// <summary>
        /// Gets the MySQL version setting from configuration.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The MySQL version.</returns>
        public static string GetMySqlVersion(this IConfiguration configuration)
        {
            return configuration.GetProperties().GetPropertyValue("MySqlVersion", "5.6");
        }

        /// <summary>
        /// Gets the MySQL server type setting from configuration.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The MySQL server type.</returns>
        public static string GetMySqlServerType(this IConfiguration configuration)
        {
            return configuration.GetProperties().GetPropertyValue("MySqlServerType", "MySql");
        }

        /// <summary>
        /// Gets the default schema setting from configuration.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The default schema.</returns>
        public static string GetDefaultSchema(this IConfiguration configuration)
        {
            return configuration.GetProperties().GetPropertyValue("DefaultSchema", "");
        }

        /// <summary>
        /// Gets a property value from a dictionary with validation and default value.
        /// </summary>
        /// <param name="properties">The properties dictionary.</param>
        /// <param name="propertyName">The name of the property to retrieve.</param>
        /// <param name="defaultValue">The default value if property is not found.</param>
        /// <param name="validValues">Optional array of valid values for validation.</param>
        /// <returns>The property value if found and valid; otherwise, the default value.</returns>
        private static string GetPropertyValue(
            this Dictionary<string, string>? properties,
            string propertyName,
            string defaultValue,
            string[]? validValues = null)
        {
            if (properties != null && properties.TryGetValue(propertyName, out string? value))
            {
                // If valid values are specified, check if the value is in the valid set
                if (validValues != null && Array.IndexOf(validValues, value) >= 0)
                {
                    return value;
                }

                // If valid values are specified but the value is not in the set, return default
                if (validValues != null)
                {
                    return defaultValue;
                }

                // If no validation is needed, return the value or default if null
                return value ?? defaultValue;
            }

            return defaultValue;
        }
    }
}