using Microsoft.Extensions.Configuration;
using SharpAbp.Abp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    /// <summary>
    /// Extension methods for IConfiguration to work with Entity Framework Core options.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Valid Oracle SQL compatibility versions.
        /// </summary>
        private static readonly HashSet<string> ValidOracleSqlCompatibilityVersions = new(StringComparer.OrdinalIgnoreCase)
        {
            "DatabaseVersion19", "DatabaseVersion21", "DatabaseVersion23"
        };

        /// <summary>
        /// Valid Oracle logon versions.
        /// </summary>
        private static readonly HashSet<string> ValidOracleLogonVersions = new(StringComparer.OrdinalIgnoreCase)
        {
            "Version8", "Version9", "Version10", "Version11", "Version12", "Version12a"
        };
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
                ValidOracleSqlCompatibilityVersions);
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
                "Version11",
                ValidOracleLogonVersions);
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
        /// <param name="validValues">Optional set of valid values for validation.</param>
        /// <returns>The property value if found and valid; otherwise, the default value.</returns>
        private static string GetPropertyValue(
            this Dictionary<string, string>? properties,
            string propertyName,
            string defaultValue,
            HashSet<string>? validValues = null)
        {
            Check.NotNullOrWhiteSpace(propertyName, nameof(propertyName));
            Check.NotNull(defaultValue, nameof(defaultValue));

            if (properties?.TryGetValue(propertyName, out string? value) == true && !string.IsNullOrWhiteSpace(value))
            {
                // If valid values are specified, check if the value is in the valid set
                if (validValues != null)
                {
                    return validValues.Contains(value) ? value : defaultValue;
                }

                // If no validation is needed, return the value
                return value;
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets a property value from a dictionary without validation.
        /// </summary>
        /// <param name="properties">The properties dictionary.</param>
        /// <param name="propertyName">The name of the property to retrieve.</param>
        /// <param name="defaultValue">The default value if property is not found.</param>
        /// <returns>The property value if found; otherwise, the default value.</returns>
        private static string GetPropertyValue(
            this Dictionary<string, string>? properties,
            string propertyName,
            string defaultValue)
        {
            return GetPropertyValue(properties, propertyName, defaultValue, null);
        }
    }
}