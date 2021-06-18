using System.Data;

namespace SharpAbp.Abp.DbConnections
{
    public interface IInternalDbConnectionCreator
    {
        DatabaseProvider DatabaseProvider { get; }

        IDbConnection Create(DbConnectionInfo dbConnectionInfo);
    }
}
