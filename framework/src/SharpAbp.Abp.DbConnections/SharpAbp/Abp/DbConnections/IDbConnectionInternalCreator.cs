using System.Data;

namespace SharpAbp.Abp.DbConnections
{
    public interface IDbConnectionInternalCreator
    {
        DatabaseProvider DatabaseProvider { get; }

        IDbConnection Create(DbConnectionInfo dbConnectionInfo);
    }
}
