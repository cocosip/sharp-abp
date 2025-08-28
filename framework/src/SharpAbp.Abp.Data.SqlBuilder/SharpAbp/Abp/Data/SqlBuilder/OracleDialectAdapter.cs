using System;
using System.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Database dialect adapter for Oracle database
    /// Provides database-specific naming conventions and SQL syntax adaptations
    /// </summary>
    [ExposeKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.Oracle)]
    public class OracleDialectAdapter : IDatabaseDialectAdapter, ITransientDependency
    {
        /// <summary>
        /// Database provider type
        /// </summary>
        public virtual DatabaseProvider DatabaseProvider => DatabaseProvider.Oracle;

        /// <summary>
        /// Normalizes table name for Oracle by wrapping it in double quotes and handling schema/prefix
        /// </summary>
        /// <param name="dbConnection">Database connection</param>
        /// <param name="dbSchema">Database schema</param>
        /// <param name="dbTablePrefix">Table prefix</param>
        /// <param name="tableName">The table name to normalize</param>
        /// <returns>The normalized table name with proper Oracle formatting</returns>
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

            // Oracle uses double quotes for case-sensitive identifiers
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
        /// Normalizes column name for Oracle by wrapping it in double quotes
        /// </summary>
        /// <param name="dbConnection">Database connection</param>
        /// <param name="columnName">The column name to normalize</param>
        /// <returns>The normalized column name wrapped in double quotes</returns>
        public virtual string? NormalizeColumnName(IDbConnection dbConnection, string? columnName)
        {
            if (columnName.IsNullOrWhiteSpace())
                return columnName;

            // Oracle uses double quotes for case-sensitive identifiers
            return $"\"{columnName}\"";
        }

        /// <summary>
        /// Normalizes parameter name for Oracle by prefixing it with ':' symbol
        /// </summary>
        /// <param name="dbConnection">Database connection</param>
        /// <param name="parameterName">The parameter name to normalize</param>
        /// <returns>The normalized parameter name prefixed with ':' symbol, or the original value if null/empty</returns>
        public virtual string? NormalizeParameterName(IDbConnection dbConnection, string? parameterName)
        {
            if (parameterName.IsNullOrWhiteSpace())
                return parameterName;

            // Oracle uses colon prefix for parameters
            return parameterName.StartsWith(":") ? parameterName : $":{parameterName}";
        }
    }
}