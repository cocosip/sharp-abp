using System;
using System.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Database dialect adapter for OpenGauss database
    /// Provides database-specific naming conventions and SQL syntax adaptations
    /// </summary>
    [ExposeKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.OpenGauss)]
    public class OpenGaussDialectAdapter : IDatabaseDialectAdapter, ITransientDependency
    {
        /// <summary>
        /// Database provider type
        /// </summary>
        public virtual DatabaseProvider DatabaseProvider => DatabaseProvider.OpenGauss;

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

            // OpenGauss (based on PostgreSQL) uses double quotes for identifiers
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
        /// Normalize column name
        /// </summary>
        /// <param name="dbConnection">Database connection</param>
        /// <param name="columnName">Column name</param>
        /// <returns>Normalized column name</returns>
        public virtual string? NormalizeColumnName(IDbConnection dbConnection, string? columnName)
        {
            if (columnName.IsNullOrWhiteSpace())
                return columnName;

            // OpenGauss uses double quotes for identifiers
            return $"\"{columnName}\"";
        }

        /// <summary>
        /// Normalize parameter name
        /// </summary>
        /// <param name="dbConnection">Database connection</param>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>Normalized parameter name</returns>
        public virtual string? NormalizeParameterName(IDbConnection dbConnection, string? parameterName)
        {
            if (parameterName.IsNullOrWhiteSpace())
                return parameterName;

            // OpenGauss (based on PostgreSQL) uses at-sign prefix for parameters in Npgsql
            return parameterName.StartsWith("@") ? parameterName : $"@{parameterName}";
        }
    }
}