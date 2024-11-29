using MySqlConnector;
using SharpAbp.Abp.Data;
using System.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnections.MySQL
{
    [ExposeKeyedService<IInternalDbConnectionCreator>(DatabaseProvider.MySql)]
    public class MySQLInternalDbConnectionCreator : IInternalDbConnectionCreator, ITransientDependency
    {
        public DatabaseProvider DatabaseProvider => DatabaseProvider.MySql;

        public virtual IDbConnection Create(DbConnectionInfo dbConnectionInfo)
        {
            return new MySqlConnection(dbConnectionInfo.ConnectionString);
        }
    }
}
