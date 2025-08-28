using System;
using System.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Database dialect adapter for SQL Server database
    /// Provides database-specific naming conventions and SQL syntax adaptations
    /// </summary>
    [ExposeKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.SqlServer)]
    public class SqlServerDialectAdapter : IDatabaseDialectAdapter, ITransientDependency
    {
        /// <summary>
        /// Database provider type
        /// </summary>
        public virtual DatabaseProvider DatabaseProvider => DatabaseProvider.SqlServer;

        /// <summary>
        /// Normalizes table name for SQL Server by wrapping it in square brackets
        /// Handles schema and table prefix if provided
        /// </summary>
        /// <param name="dbConnection">Database connection</param>
        /// <param name="dbSchema">Database schema</param>
        /// <param name="dbTablePrefix">Table prefix</param>
        /// <param name="tableName">The table name to normalize</param>
        /// <returns>The normalized table name wrapped in square brackets with schema if provided</returns>
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

            // SQL Server uses square brackets for identifiers
            // If schema is provided, format as [Schema].[Table]
            if (!actualSchema.IsNullOrWhiteSpace())
            {
                return $"[{actualSchema}].[{finalTableName}]";
            }
            else
            {
                return $"[{finalTableName}]";
            }
        }

        /// <summary>
        /// Normalizes column name for SQL Server by wrapping it in square brackets
        /// </summary>
        /// <param name="dbConnection">Database connection</param>
        /// <param name="columnName">The column name to normalize</param>
        /// <returns>The normalized column name wrapped in square brackets, or the original value if null/empty</returns>
        public virtual string? NormalizeColumnName(IDbConnection dbConnection, string? columnName)
        {
            if (columnName.IsNullOrWhiteSpace())
                return columnName;

            // SQL Server uses square brackets for identifiers
            return $"[{columnName}]";
        }

        /// <summary>
        /// Normalizes parameter name for SQL Server by prefixing it with '@' symbol
        /// </summary>
        /// <param name="dbConnection">Database connection</param>
        /// <param name="parameterName">The parameter name to normalize</param>
        /// <returns>The normalized parameter name prefixed with '@' symbol, or the original value if null/empty</returns>
        public virtual string? NormalizeParameterName(IDbConnection dbConnection, string? parameterName)
        {
            if (parameterName.IsNullOrWhiteSpace())
                return parameterName;

            // SQL Server uses at-sign prefix for parameters
            return parameterName.StartsWith("@") ? parameterName : $"@{parameterName}";
        }
    }
}