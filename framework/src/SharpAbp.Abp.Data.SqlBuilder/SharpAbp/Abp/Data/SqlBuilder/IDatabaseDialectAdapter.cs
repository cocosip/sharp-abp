using System.Data;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    public interface IDatabaseDialectAdapter
    {
        DatabaseProvider DatabaseProvider { get; }
        string? NormalizeTableName(IDbConnection dbConnection, string? dbSchema, string? dbTablePrefix, string? tableName);
        string? NormalizeColumnName(IDbConnection dbConnection, string? columnName);
        string? NormalizeParameterName(IDbConnection dbConnection, string? parameterName);
    }
}