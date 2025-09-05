using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    /// <summary>
    /// Constants for Entity Framework Core configuration.
    /// </summary>
    public static class EfCoreConstants
    {
        /// <summary>
        /// Property name constants for EF Core configuration.
        /// </summary>
        public static class PropertyNames
        {
            /// <summary>
            /// PostgreSQL version property name.
            /// </summary>
            public const string PostgreSqlVersion = "PostgreSqlVersion";

            /// <summary>
            /// Oracle SQL compatibility property name.
            /// </summary>
            public const string OracleSQLCompatibility = "OracleSQLCompatibility";

            /// <summary>
            /// Oracle allowed logon version client property name.
            /// </summary>
            public const string OracleAllowedLogonVersionClient = "OracleAllowedLogonVersionClient";

            /// <summary>
            /// MySQL version property name.
            /// </summary>
            public const string MySqlVersion = "MySqlVersion";

            /// <summary>
            /// MySQL server type property name.
            /// </summary>
            public const string MySqlServerType = "MySqlServerType";

            /// <summary>
            /// Default schema property name.
            /// </summary>
            public const string DefaultSchema = "DefaultSchema";
        }

        /// <summary>
        /// Default values for EF Core configuration properties.
        /// </summary>
        public static class DefaultValues
        {
            /// <summary>
            /// Default Oracle SQL compatibility version.
            /// </summary>
            public const string OracleSQLCompatibility = "DatabaseVersion19";

            /// <summary>
            /// Default Oracle allowed logon version.
            /// </summary>
            public const string OracleAllowedLogonVersionClient = "Version11";

            /// <summary>
            /// Default MySQL version.
            /// </summary>
            public const string MySqlVersion = "5.6";

            /// <summary>
            /// Default MySQL server type.
            /// </summary>
            public const string MySqlServerType = "MySql";
        }

        /// <summary>
        /// Valid Oracle SQL compatibility versions.
        /// </summary>
        public static readonly HashSet<string> ValidOracleSqlCompatibilityVersions = new(StringComparer.OrdinalIgnoreCase)
        {
            "DatabaseVersion19",
            "DatabaseVersion21",
            "DatabaseVersion23"
        };
        
        /// <summary>
        /// Valid Oracle allowed logon versions for clients.
        /// </summary>
        public static readonly HashSet<string> ValidOracleLogonVersions = new(StringComparer.OrdinalIgnoreCase)
        {
            "Version8",
            "Version9",
            "Version10",
            "Version11",
            "Version12",
            "Version12a"
        };
    }
}