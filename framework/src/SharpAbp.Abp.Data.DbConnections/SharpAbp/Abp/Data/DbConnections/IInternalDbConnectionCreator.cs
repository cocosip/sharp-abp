using System.Data;

namespace SharpAbp.Abp.Data.DbConnections
{
    public interface IInternalDbConnectionCreator
    {
        DatabaseProvider DatabaseProvider { get; }

        IDbConnection Create(DbConnectionInfo dbConnectionInfo);
    }
}
