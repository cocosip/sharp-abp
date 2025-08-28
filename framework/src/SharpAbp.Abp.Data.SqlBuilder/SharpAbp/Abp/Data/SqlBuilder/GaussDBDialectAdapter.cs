using System;
using System.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Database dialect adapter for GaussDB database
    /// Provides database-specific naming conventions and SQL syntax adaptations
    /// </summary>
    [ExposeKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.GaussDB)]
    public class GaussDBDialectAdapter : IDatabaseDialectAdapter, ITransientDependency
    {
        /// <summary>
        /// Database provider type
        /// </summary>
        public virtual DatabaseProvider DatabaseProvider => DatabaseProvider.GaussDB;

        /// <summary>
        /// Normalize table name with schema and prefix
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

            // GaussDB (based on PostgreSQL) uses double quotes for identifiers
            // If schema is provided, format as "Schema"."Table"
            if (!actualSchema.IsNullOrWhiteSpace())
            {
                return $"\"{actualSchema}\".\"{finalTableName}\"";
            }
            else
            {
                return $"\"{finalTableName}\"";
            }
        }

        /// <summary>
        /// Normalizes column name for GaussDB by wrapping it in double quotes
        /// </summary>
        /// <param name="dbConnection">Database connection</param>
        /// <param name="columnName">The column name to normalize</param>
        /// <returns>The normalized column name wrapped in double quotes, or the original value if null/empty</returns>
        public virtual string? NormalizeColumnName(IDbConnection dbConnection, string? columnName)
        {
            // GaussDB (based on PostgreSQL) uses double quotes for identifiers
            if (columnName.IsNullOrWhiteSpace())
                return columnName;
            return $"\"{columnName}\"";
        }

        /// <summary>
        /// Normalizes parameter name for GaussDB by prefixing it with '@' symbol
        /// </summary>
        /// <param name="dbConnection">Database connection</param>
        /// <param name="parameterName">The parameter name to normalize</param>
        /// <returns>The normalized parameter name prefixed with '@' symbol, or the original value if null/empty</returns>
        public virtual string? NormalizeParameterName(IDbConnection dbConnection, string? parameterName)
        {
            if (parameterName.IsNullOrWhiteSpace())
                return parameterName;

            // GaussDB (based on PostgreSQL) uses at-sign prefix for parameters in Npgsql
            return parameterName.StartsWith("@") ? parameterName : $"@{parameterName}";
        }
    }
}