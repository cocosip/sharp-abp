using System;
using System.Data;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Database dialect adapter for DM (Dameng) database
    /// Provides database-specific naming conventions and SQL syntax adaptations
    /// Supports Oracle, PostgreSQL, and MySQL compatibility modes
    /// </summary>
    [ExposeKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.Dm)]
    public class DmDialectAdapter : IDatabaseDialectAdapter, ITransientDependency
    {

        /// <summary>
        /// Logger instance
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// DM database mode detector
        /// </summary>
        protected IDmDatabaseModeDetector ModeDetector { get; }

        /// <summary>
        /// Initializes a new instance of the DmDialectAdapter class
        /// </summary>
        /// <param name="logger">Logger instance</param>
        /// <param name="modeDetector">DM database mode detector</param>
        public DmDialectAdapter(
            ILogger<DmDialectAdapter> logger,
            IDmDatabaseModeDetector modeDetector)
        {
            Logger = logger;
            ModeDetector = modeDetector;
        }

        /// <summary>
        /// Database provider type
        /// </summary>
        public virtual DatabaseProvider DatabaseProvider => DatabaseProvider.Dm;

        /// <summary>
        /// Normalize table name with schema and prefix based on DM database compatibility mode
        /// </summary>
        /// <param name="dbConnection">Database connection</param>
        /// <param name="dbSchema">Database schema</param>
        /// <param name="dbTablePrefix">Table prefix</param>
        /// <param name="tableName">Table name</param>
        /// <returns>Normalized table name</returns>
        public virtual string? NormalizeTableName(IDbConnection dbConnection, string? dbSchema, string? dbTablePrefix, string? tableName)
        {
            if (tableName.IsNullOrWhiteSpace())
                return tableName;

            var mode = GetDatabaseMode(dbConnection);
            var finalTableName = tableName;
            var finalSchema = dbSchema;

            // Check if tableName contains schema (e.g., "HR.EMPLOYEES")
            if (tableName.Contains(".") && dbSchema.IsNullOrWhiteSpace())
            {
                var parts = tableName.Split('.');
                if (parts.Length == 2)
                {
                    finalSchema = parts[0];
                    finalTableName = parts[1];
                }
            }

            if (!dbTablePrefix.IsNullOrWhiteSpace() && !finalTableName.StartsWith(dbTablePrefix, StringComparison.OrdinalIgnoreCase))
            {
                finalTableName = $"{dbTablePrefix}{finalTableName}";
            }

            return mode switch
            {
                DmDatabaseMode.Oracle => NormalizeTableNameForOracle(finalSchema, finalTableName),
                DmDatabaseMode.PostgreSql => NormalizeTableNameForPostgreSql(finalSchema, finalTableName),
                DmDatabaseMode.MySql => NormalizeTableNameForMySql(finalSchema, finalTableName),
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unsupported DM database mode")
            };
        }

        /// <summary>
        /// Normalizes column name for DM database based on compatibility mode
        /// </summary>
        /// <param name="dbConnection">Database connection</param>
        /// <param name="columnName">The column name to normalize</param>
        /// <returns>The normalized column name, or the original value if null/empty</returns>
        public virtual string? NormalizeColumnName(IDbConnection dbConnection, string? columnName)
        {
            if (columnName.IsNullOrWhiteSpace())
                return columnName;

            var mode = GetDatabaseMode(dbConnection);

            return mode switch
            {
                DmDatabaseMode.Oracle => $"\"{columnName}\"", // Oracle uses double quotes
                DmDatabaseMode.PostgreSql => $"\"{columnName}\"", // PostgreSQL uses double quotes
                DmDatabaseMode.MySql => $"`{columnName}`", // MySQL uses backticks
                _ => $"\"{columnName}\"" // Default to Oracle style
            };
        }

        /// <summary>
        /// Normalizes parameter name for DM database based on compatibility mode
        /// </summary>
        /// <param name="dbConnection">Database connection</param>
        /// <param name="parameterName">The parameter name to normalize</param>
        /// <returns>The normalized parameter name, or the original value if null/empty</returns>
        public virtual string? NormalizeParameterName(IDbConnection dbConnection, string? parameterName)
        {
            if (parameterName.IsNullOrWhiteSpace())
                return parameterName;

            var mode = GetDatabaseMode(dbConnection);

            return mode switch
            {
                DmDatabaseMode.Oracle => parameterName.StartsWith(":") ? parameterName : $":{parameterName}",
                DmDatabaseMode.PostgreSql => parameterName.StartsWith("@") ? parameterName : $"@{parameterName}",
                DmDatabaseMode.MySql => parameterName.StartsWith("?") ? parameterName : $"?{parameterName}",
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unsupported DM database mode")
            };
        }

        /// <summary>
        /// Gets the database mode for the given connection
        /// </summary>
        /// <param name="dbConnection">The database connection</param>
        /// <returns>The detected database mode</returns>
        private DmDatabaseMode GetDatabaseMode(IDbConnection dbConnection)
        {
            return ModeDetector.DetectMode(dbConnection);
        }

        /// <summary>
        /// Normalizes table name for Oracle compatibility mode
        /// </summary>
        /// <param name="dbSchema">Database schema</param>
        /// <param name="tableName">Table name</param>
        /// <returns>Normalized table name</returns>
        protected virtual string? NormalizeTableNameForOracle(string? dbSchema, string? tableName)
        {
            if (!dbSchema.IsNullOrWhiteSpace())
            {
                return $"\"{dbSchema}\".\"{tableName}\"";
            }
            return $"\"{tableName}\"";
        }

        /// <summary>
        /// Normalizes table name for PostgreSQL compatibility mode
        /// </summary>
        /// <param name="dbSchema">Database schema</param>
        /// <param name="tableName">Table name</param>
        /// <returns>Normalized table name</returns>
        protected virtual string? NormalizeTableNameForPostgreSql(string? dbSchema, string? tableName)
        {
            if (!dbSchema.IsNullOrWhiteSpace())
            {
                return $"\"{dbSchema}\".\"{tableName}\"";
            }
            return $"\"{tableName}\"";
        }

        /// <summary>
        /// Normalizes table name for MySQL compatibility mode
        /// </summary>
        /// <param name="dbSchema">Database schema</param>
        /// <param name="tableName">Table name</param>
        /// <returns>Normalized table name</returns>
        protected virtual string? NormalizeTableNameForMySql(string? dbSchema, string? tableName)
        {
            if (!dbSchema.IsNullOrWhiteSpace())
            {
                return $"`{dbSchema}`.`{tableName}`";
            }
            return $"`{tableName}`";
        }
    }
}