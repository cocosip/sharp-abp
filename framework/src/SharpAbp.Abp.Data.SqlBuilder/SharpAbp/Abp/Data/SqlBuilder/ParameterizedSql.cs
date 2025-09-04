using System.Collections.Generic;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    public class ParameterizedSql
    {
        /// <summary>
        /// Gets or sets the generated SQL query string.
        /// </summary>
        public string? Sql { get; set; }

        /// <summary>
        /// Gets or sets the parameters for the SQL query.
        /// </summary>
        public Dictionary<string, object>? Parameters { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterizedSql"/> class.
        /// </summary>
        public ParameterizedSql()
        {
            Parameters = [];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterizedSql"/> class with specified SQL and parameters.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters for the SQL query.</param>
        public ParameterizedSql(string sql, Dictionary<string, object>? parameters = null)
        {
            Sql = sql;
            Parameters = parameters ?? [];
        }
    }
}