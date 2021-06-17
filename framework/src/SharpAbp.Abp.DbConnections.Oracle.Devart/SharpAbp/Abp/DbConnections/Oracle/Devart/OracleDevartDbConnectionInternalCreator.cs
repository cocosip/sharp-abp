using Devart.Data.Oracle;
using System.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnections.Oracle.Devart
{
    public class OracleDbConnectionInternalCreator : IDbConnectionInternalCreator, ITransientDependency
    {
        public DatabaseProvider DatabaseProvider => DatabaseProvider.Oracle;

        public virtual IDbConnection Create(DbConnectionInfo dbConnectionInfo)
        {
            return new OracleConnection(dbConnectionInfo.ConnectionString);
        }
    }
}
