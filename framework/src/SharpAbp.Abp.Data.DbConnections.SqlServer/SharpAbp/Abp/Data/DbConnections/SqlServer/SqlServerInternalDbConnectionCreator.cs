using Microsoft.Data.SqlClient;
using System.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Data.DbConnections.SqlServer
{
    public class SqlServerInternalDbConnectionCreator : IInternalDbConnectionCreator, ITransientDependency
    {
        public DatabaseProvider DatabaseProvider => DatabaseProvider.SqlServer;

        public virtual IDbConnection Create(DbConnectionInfo dbConnectionInfo)
        {
            return new SqlConnection(dbConnectionInfo.ConnectionString);
        }
    }
}
