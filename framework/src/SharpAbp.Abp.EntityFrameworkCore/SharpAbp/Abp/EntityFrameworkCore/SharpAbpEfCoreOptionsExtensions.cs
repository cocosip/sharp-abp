using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    /// <summary>
    /// Extension methods for SharpAbpEfCoreOptions.
    /// </summary>
    public static class SharpAbpEfCoreOptionsExtensions
    {
        #region Constants
        
        /// <summary>
        /// Valid Oracle SQL compatibility versions.
        /// </summary>
        private static readonly HashSet<string> ValidOracleSqlCompatibilityVersions = new(StringComparer.OrdinalIgnoreCase)
        {
            "DatabaseVersion19",
            "DatabaseVersion21",
            "DatabaseVersion23"
        };
        
        /// <summary>
        /// Valid Oracle allowed logon versions for clients.
        /// </summary>
        private static readonly HashSet<string> ValidOracleLogonVersions = new(StringComparer.OrdinalIgnoreCase)
        {
            "Version8",
            "Version9",
            "Version10",
            "Version11",
            "Version12",
            "Version12a"
        };
        
        #endregion
        
        /// <summary>
        /// Gets the Oracle SQL compatibility version.
        /// </summary>
        /// <param name="options">The SharpAbpEfCoreOptions instance.</param>
        /// <returns>The Oracle SQL compatibility version, or "DatabaseVersion19" if not specified or invalid.</returns>
        public static string GetOracleSQLCompatibility(this SharpAbpEfCoreOptions options)
        {
            return GetPropertyValue(options, "OracleSQLCompatibility", "DatabaseVersion19", ValidOracleSqlCompatibilityVersions);
        }

        /// <summary>
        /// Gets the Oracle allowed logon version for clients.
        /// </summary>
        /// <param name="options">The SharpAbpEfCoreOptions instance.</param>
        /// <returns>The Oracle allowed logon version, or "Version11" if not specified or invalid.</returns>
        public static string GetOracleAllowedLogonVersionClient(this SharpAbpEfCoreOptions options)
        {
            return GetPropertyValue(options, "OracleAllowedLogonVersionClient", "Version11", ValidOracleLogonVersions);
        }

        /// <summary>
        /// Gets the MySQL version.
        /// </summary>
        /// <param name="options">The SharpAbpEfCoreOptions instance.</param>
        /// <returns>The MySQL version, or "5.6" if not specified.</returns>
        public static string GetMySqlVersion(this SharpAbpEfCoreOptions options)
        {
            return GetPropertyValue(options, "MySqlVersion", "5.6");
        }

        /// <summary>
        /// Gets the MySQL server type.
        /// </summary>
        /// <param name="options">The SharpAbpEfCoreOptions instance.</param>
        /// <returns>The MySQL server type, or "MySql" if not specified.</returns>
        public static string GetMySqlServerType(this SharpAbpEfCoreOptions options)
        {
            return GetPropertyValue(options, "MySqlServerType", "MySql");
        }

        /// <summary>
        /// Gets the default schema.
        /// </summary>
        /// <param name="options">The SharpAbpEfCoreOptions instance.</param>
        /// <returns>The default schema, or an empty string if not specified.</returns>
        public static string GetDefaultSchema(this SharpAbpEfCoreOptions options)
        {
            return GetPropertyValue(options, "DefaultSchema", "");
        }

        /// <summary>
        /// Gets a property value from the options Properties dictionary with validation.
        /// </summary>
        /// <param name="options">The SharpAbpEfCoreOptions instance.</param>
        /// <param name="propertyName">The name of the property to retrieve.</param>
        /// <param name="defaultValue">The default value to return if the property is not found.</param>
        /// <param name="validValues">Optional set of valid values for validation.</param>
        /// <returns>The property value if found and valid; otherwise, the default value.</returns>
        private static string GetPropertyValue(
            SharpAbpEfCoreOptions options,
            string propertyName,
            string defaultValue = "",
            HashSet<string>? validValues = null)
        {
            Check.NotNull(options, nameof(options));
            Check.NotNullOrWhiteSpace(propertyName, nameof(propertyName));
            
            if (!options.Properties.TryGetValue(propertyName, out string? value) || string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            // If valid values are specified, check if the value is in the valid set
            if (validValues?.Count > 0)
            {
                return validValues.Contains(value) ? value : defaultValue;
            }

            return value;
        }
        
        /// <summary>
        /// Gets a property value from the options Properties dictionary without validation.
        /// </summary>
        /// <param name="options">The SharpAbpEfCoreOptions instance.</param>
        /// <param name="propertyName">The name of the property to retrieve.</param>
        /// <param name="defaultValue">The default value to return if the property is not found.</param>
        /// <returns>The property value if found; otherwise, the default value.</returns>
        private static string GetPropertyValue(
            SharpAbpEfCoreOptions options,
            string propertyName,
            string defaultValue)
        {
            return GetPropertyValue(options, propertyName, defaultValue, null);
        }
    }
}
