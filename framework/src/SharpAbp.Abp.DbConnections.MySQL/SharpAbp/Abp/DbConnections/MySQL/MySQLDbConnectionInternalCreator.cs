using MySqlConnector;
using System.Data;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnections.MySQL
{
    public class MySQLDbConnectionInternalCreator : IDbConnectionInternalCreator, ITransientDependency
    {
        public DatabaseProvider DatabaseProvider => DatabaseProvider.MySql;

        public virtual IDbConnection Create(DbConnectionInfo dbConnectionInfo)
        {
            Check.NotNullOrWhiteSpace(dbConnectionInfo.ConnectionString, nameof(dbConnectionInfo.ConnectionString));

            return new MySqlConnection(dbConnectionInfo.ConnectionString);
        }
    }
}
