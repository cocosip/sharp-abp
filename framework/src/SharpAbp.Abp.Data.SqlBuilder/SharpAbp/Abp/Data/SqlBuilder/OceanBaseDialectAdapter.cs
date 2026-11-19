using System;
using System.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Database dialect adapter for OceanBase database.
    /// An OceanBase tenant runs in either Oracle or MySQL compatibility mode, so identifiers and
    /// parameter markers follow the mode of the given connection.
    /// <see cref="DatabaseProvider.OceanBase"/> defaults to Oracle mode when the mode cannot be read.
    /// </summary>
    [ExposeKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.OceanBase)]
    public class OceanBaseDialectAdapter : IDatabaseDialectAdapter, ITransientDependency
    {
        /// <summary>
        /// OceanBase database mode detector
        /// </summary>
        protected IOceanBaseDatabaseModeDetector ModeDetector { get; }

        public OceanBaseDialectAdapter(IOceanBaseDatabaseModeDetector modeDetector)
        {
            ModeDetector = modeDetector;
        }

        /// <summary>
        /// Database provider type
        /// </summary>
        public virtual DatabaseProvider DatabaseProvider => DatabaseProvider.OceanBase;

        /// <summary>
        /// Normalizes table name with schema and prefix based on the OceanBase compatibility mode
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

            var actualSchema = dbSchema;
            var actualTableName = tableName;

            if (tableName.Contains(".") && dbSchema.IsNullOrWhiteSpace())
            {
                var parts = tableName.Split(['.'], 2);
                if (parts.Length == 2)
                {
                    actualSchema = parts[0];
                    actualTableName = parts[1];
                }
            }

            if (!dbTablePrefix.IsNullOrWhiteSpace() && !actualTableName.StartsWith(dbTablePrefix, StringComparison.OrdinalIgnoreCase))
            {
                actualTableName = $"{dbTablePrefix}{actualTableName}";
            }

            var mode = GetDatabaseMode(dbConnection);
            return mode switch
            {
                OceanBaseDatabaseMode.Oracle => NormalizeTableNameForOracle(actualSchema, actualTableName),
                OceanBaseDatabaseMode.MySql => NormalizeTableNameForMySql(actualSchema, actualTableName),
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unsupported OceanBase database mode")
            };
        }

        /// <summary>
        /// Normalizes column name based on the OceanBase compatibility mode
        /// </summary>
        /// <param name="dbConnection">Database connection</param>
        /// <param name="columnName">Column name</param>
        /// <returns>Normalized column name, or the original value if null/empty</returns>
        public virtual string? NormalizeColumnName(IDbConnection dbConnection, string? columnName)
        {
            if (columnName.IsNullOrWhiteSpace())
                return columnName;

            var mode = GetDatabaseMode(dbConnection);
            return mode switch
            {
                OceanBaseDatabaseMode.Oracle => $"\"{columnName}\"",
                OceanBaseDatabaseMode.MySql => $"`{columnName}`",
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unsupported OceanBase database mode")
            };
        }

        /// <summary>
        /// Normalizes parameter name based on the OceanBase compatibility mode
        /// </summary>
        /// <param name="dbConnection">Database connection</param>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>
        /// Oracle mode returns the colon prefixed name. MySQL mode returns a positional marker,
        /// because the OceanBase connector binds MySQL parameters by position and sends the SQL as is.
        /// </returns>
        public virtual string? NormalizeParameterName(IDbConnection dbConnection, string? parameterName)
        {
            if (parameterName.IsNullOrWhiteSpace())
                return parameterName;

            var mode = GetDatabaseMode(dbConnection);
            return mode switch
            {
                OceanBaseDatabaseMode.Oracle => parameterName.StartsWith(":") ? parameterName : $":{parameterName}",
                OceanBaseDatabaseMode.MySql => "?",
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unsupported OceanBase database mode")
            };
        }

        /// <summary>
        /// Gets the database mode for the given connection
        /// </summary>
        /// <param name="dbConnection">The database connection</param>
        /// <returns>The detected database mode</returns>
        private OceanBaseDatabaseMode GetDatabaseMode(IDbConnection dbConnection)
        {
            return ModeDetector.DetectMode(dbConnection);
        }

        /// <summary>
        /// Normalizes table name for Oracle compatibility mode
        /// </summary>
        /// <param name="dbSchema">Database schema</param>
        /// <param name="tableName">Table name</param>
        /// <returns>Normalized table name</returns>
        protected virtual string NormalizeTableNameForOracle(string? dbSchema, string tableName)
        {
            return dbSchema.IsNullOrWhiteSpace()
                ? $"\"{tableName}\""
                : $"\"{dbSchema}\".\"{tableName}\"";
        }

        /// <summary>
        /// Normalizes table name for MySQL compatibility mode
        /// </summary>
        /// <param name="dbSchema">Database schema</param>
        /// <param name="tableName">Table name</param>
        /// <returns>Normalized table name</returns>
        protected virtual string NormalizeTableNameForMySql(string? dbSchema, string tableName)
        {
            return dbSchema.IsNullOrWhiteSpace()
                ? $"`{tableName}`"
                : $"`{dbSchema}`.`{tableName}`";
        }
    }
}
