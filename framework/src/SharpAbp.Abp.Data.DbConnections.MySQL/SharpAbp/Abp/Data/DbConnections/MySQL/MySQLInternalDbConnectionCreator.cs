using MySqlConnector;
using System.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Data.DbConnections.MySQL
{
    public class MySQLInternalDbConnectionCreator : IInternalDbConnectionCreator, ITransientDependency
    {
        public DatabaseProvider DatabaseProvider => DatabaseProvider.MySql;

        public virtual IDbConnection Create(DbConnectionInfo dbConnectionInfo)
        {
            return new MySqlConnection(dbConnectionInfo.ConnectionString);
        }
    }
}
