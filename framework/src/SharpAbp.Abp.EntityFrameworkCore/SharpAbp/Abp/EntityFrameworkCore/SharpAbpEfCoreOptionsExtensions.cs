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
        
        /// <summary>
        /// Gets the PostgreSQL version.
        /// </summary>
        /// <param name="options">The SharpAbpEfCoreOptions instance.</param>
        /// <returns>The PostgreSQL version, or an empty string if not specified.</returns>
        public static string GetPostgreSqlVersion(this SharpAbpEfCoreOptions options)
        {
            return options.GetPropertyValue(EfCoreConstants.PropertyNames.PostgreSqlVersion, "");
        }

        /// <summary>
        /// Gets the Oracle SQL compatibility version.
        /// </summary>
        /// <param name="options">The SharpAbpEfCoreOptions instance.</param>
        /// <returns>The Oracle SQL compatibility version, or "DatabaseVersion19" if not specified or invalid.</returns>
        public static string GetOracleSQLCompatibility(this SharpAbpEfCoreOptions options)
        {
            return options.GetPropertyValue(
                EfCoreConstants.PropertyNames.OracleSQLCompatibility,
                EfCoreConstants.DefaultValues.OracleSQLCompatibility,
                EfCoreConstants.ValidOracleSqlCompatibilityVersions);
        }

        /// <summary>
        /// Gets the Oracle allowed logon version for clients.
        /// </summary>
        /// <param name="options">The SharpAbpEfCoreOptions instance.</param>
        /// <returns>The Oracle allowed logon version, or "Version11" if not specified or invalid.</returns>
        public static string GetOracleAllowedLogonVersionClient(this SharpAbpEfCoreOptions options)
        {
            return options.GetPropertyValue(
                EfCoreConstants.PropertyNames.OracleAllowedLogonVersionClient,
                EfCoreConstants.DefaultValues.OracleAllowedLogonVersionClient,
                EfCoreConstants.ValidOracleLogonVersions);
        }

        /// <summary>
        /// Gets the MySQL version.
        /// </summary>
        /// <param name="options">The SharpAbpEfCoreOptions instance.</param>
        /// <returns>The MySQL version, or "5.6" if not specified.</returns>
        public static string GetMySqlVersion(this SharpAbpEfCoreOptions options)
        {
            return options.GetPropertyValue(
                EfCoreConstants.PropertyNames.MySqlVersion, 
                EfCoreConstants.DefaultValues.MySqlVersion);
        }

        /// <summary>
        /// Gets the MySQL server type.
        /// </summary>
        /// <param name="options">The SharpAbpEfCoreOptions instance.</param>
        /// <returns>The MySQL server type, or "MySql" if not specified.</returns>
        public static string GetMySqlServerType(this SharpAbpEfCoreOptions options)
        {
            return options.GetPropertyValue(
                EfCoreConstants.PropertyNames.MySqlServerType, 
                EfCoreConstants.DefaultValues.MySqlServerType);
        }

        /// <summary>
        /// Gets the default schema.
        /// </summary>
        /// <param name="options">The SharpAbpEfCoreOptions instance.</param>
        /// <returns>The default schema, or an empty string if not specified.</returns>
        public static string GetDefaultSchema(this SharpAbpEfCoreOptions options)
        {
            return options.GetPropertyValue(EfCoreConstants.PropertyNames.DefaultSchema, "");
        }


    }
}
