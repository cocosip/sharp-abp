using Oracle.ManagedDataAccess.Client;
using System.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Data.DbConnections.Oracle
{
    public class OracleInternalDbConnectionCreator : IInternalDbConnectionCreator, ITransientDependency
    {
        public DatabaseProvider DatabaseProvider => DatabaseProvider.Oracle;

        public virtual IDbConnection Create(DbConnectionInfo dbConnectionInfo)
        {
            return new OracleConnection(dbConnectionInfo.ConnectionString);
        }
    }
}
