using System;
using System.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Database dialect adapter for MySQL database
    /// Provides database-specific naming conventions and SQL syntax adaptations
    /// </summary>
    [ExposeKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.MySql)]
    public class MySqlDialectAdapter : IDatabaseDialectAdapter, ITransientDependency
    {
        /// <summary>
        /// Database provider type
        /// </summary>
        public virtual DatabaseProvider DatabaseProvider => DatabaseProvider.MySql;

        /// <summary>
        /// Normalizes table name for MySQL by wrapping it in backticks with schema and prefix support
        /// </summary>
        /// <param name="dbConnection">Database connection</param>
        /// <param name="dbSchema">Database schema</param>
        /// <param name="dbTablePrefix">Table prefix</param>
        /// <param name="tableName">Table name</param>
        /// <returns>Normalized table name wrapped in backticks with schema and prefix applied</returns>
        public virtual string? NormalizeTableName(IDbConnection dbConnection, string? dbSchema, string? dbTablePrefix, string? tableName)
        {
            if (tableName.IsNullOrWhiteSpace())
                return tableName;

            // Parse schema and table name from input if it contains a dot
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

            var finalTableName = actualTableName;
            if (!dbTablePrefix.IsNullOrWhiteSpace() && !actualTableName.StartsWith(dbTablePrefix, StringComparison.OrdinalIgnoreCase))
            {
                finalTableName = $"{dbTablePrefix}{actualTableName}";
            }

            // MySQL uses backticks for identifiers
            // If schema is provided, format as `Schema`.`Table`
            if (!actualSchema.IsNullOrWhiteSpace())
            {
                return $"`{actualSchema}`.`{finalTableName}`";
            }
            else
            {
                return $"`{finalTableName}`";
            }
        }

        /// <summary>
        /// Normalizes column name for MySQL by wrapping it in backticks
        /// </summary>
        /// <param name="dbConnection">Database connection</param>
        /// <param name="columnName">Column name</param>
        /// <returns>Normalized column name wrapped in backticks, or the original value if null/empty</returns>
        public virtual string? NormalizeColumnName(IDbConnection dbConnection, string? columnName)
        {
            if (columnName.IsNullOrWhiteSpace())
                return columnName;

            // MySQL uses backticks for identifiers
            return $"`{columnName}`";
        }

        /// <summary>
        /// Normalizes parameter name for MySQL by prefixing it with '@' symbol
        /// </summary>
        /// <param name="dbConnection">Database connection</param>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>Normalized parameter name prefixed with '@' symbol, or the original value if null/empty</returns>
        public virtual string? NormalizeParameterName(IDbConnection dbConnection, string? parameterName)
        {
            if (parameterName.IsNullOrWhiteSpace())
                return parameterName;

            // MySQL uses at-sign prefix for parameters
            return parameterName.StartsWith("@") ? parameterName : $"@{parameterName}";
        }
    }
}