using System;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    /// <summary>
    /// Extension methods for SharpAbpEfCoreOptions.
    /// </summary>
    public static class SharpAbpEfCoreOptionsExtensions
    {
        /// <summary>
        /// Gets the Oracle SQL compatibility version.
        /// </summary>
        /// <param name="options">The SharpAbpEfCoreOptions instance.</param>
        /// <returns>The Oracle SQL compatibility version, or "DatabaseVersion19" if not specified or invalid.</returns>
        public static string GetOracleSQLCompatibility(this SharpAbpEfCoreOptions options)
        {
            string[] validValues = { "DatabaseVersion19", "DatabaseVersion21", "DatabaseVersion23" };
            return GetPropertyValue(options, "OracleSQLCompatibility", "DatabaseVersion19", validValues);
        }

        /// <summary>
        /// Gets the Oracle allowed logon version for clients.
        /// </summary>
        /// <param name="options">The SharpAbpEfCoreOptions instance.</param>
        /// <returns>The Oracle allowed logon version, or "DatabaseVersion19" if not specified or invalid.</returns>
        public static string GetOracleAllowedLogonVersionClient(this SharpAbpEfCoreOptions options)
        {
            string[] validValues = ["Version8", "Version9", "Version10", "Version11", "Version12", "Version12a"];
            return GetPropertyValue(options, "OracleAllowedLogonVersionClient", "DatabaseVersion19", validValues);
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
        /// <param name="validValues">Optional array of valid values for validation.</param>
        /// <returns>The property value if found and valid; otherwise, the default value.</returns>
        private static string GetPropertyValue(
            SharpAbpEfCoreOptions options,
            string propertyName,
            string defaultValue = "",
            string[]? validValues = null)
        {
            if (options.Properties.TryGetValue(propertyName, out string? value))
            {
                // If valid values are specified, check if the value is in the valid set
                if (validValues != null && validValues.Length > 0)
                {
                    foreach (string validValue in validValues)
                    {
                        if (string.Equals(value, validValue, StringComparison.OrdinalIgnoreCase))
                        {
                            return value;
                        }
                    }
                    // If we get here, the value is not in the valid set, so return the default
                    return defaultValue;
                }

                // If no validation is needed, just return the value or default
                return value ?? defaultValue;
            }

            return defaultValue;
        }
    }
}
