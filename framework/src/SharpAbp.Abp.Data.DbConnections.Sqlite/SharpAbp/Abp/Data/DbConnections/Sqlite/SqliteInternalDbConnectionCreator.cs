using Microsoft.Data.Sqlite;
using System.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Data.DbConnections.Sqlite
{
    public class SqliteInternalDbConnectionCreator : IInternalDbConnectionCreator, ITransientDependency
    {
        public DatabaseProvider DatabaseProvider => DatabaseProvider.Sqlite;

        public virtual IDbConnection Create(DbConnectionInfo dbConnectionInfo)
        {
            return new SqliteConnection(dbConnectionInfo.ConnectionString);
        }
    }
}
