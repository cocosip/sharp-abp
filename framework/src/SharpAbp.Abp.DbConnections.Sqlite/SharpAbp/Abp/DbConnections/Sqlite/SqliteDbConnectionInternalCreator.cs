using Microsoft.Data.Sqlite;
using System.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnections.Sqlite
{
    public class SqliteDbConnectionInternalCreator : IDbConnectionInternalCreator, ITransientDependency
    {
        public DatabaseProvider DatabaseProvider => DatabaseProvider.Sqlite;

        public virtual IDbConnection Create(DbConnectionInfo dbConnectionInfo)
        {
            return new SqliteConnection(dbConnectionInfo.ConnectionString);
        }
    }
}
