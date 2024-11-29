using Microsoft.Data.Sqlite;
using SharpAbp.Abp.Data;
using System.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnections.Sqlite
{
    [ExposeKeyedService<IInternalDbConnectionCreator>(DatabaseProvider.Sqlite)]
    public class SqliteInternalDbConnectionCreator : IInternalDbConnectionCreator, ITransientDependency
    {
        public DatabaseProvider DatabaseProvider => DatabaseProvider.Sqlite;

        public virtual IDbConnection Create(DbConnectionInfo dbConnectionInfo)
        {
            return new SqliteConnection(dbConnectionInfo.ConnectionString);
        }
    }
}
