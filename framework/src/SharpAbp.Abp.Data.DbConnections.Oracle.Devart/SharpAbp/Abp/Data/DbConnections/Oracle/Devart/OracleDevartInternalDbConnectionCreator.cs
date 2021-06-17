using Devart.Data.Oracle;
using System.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Data.DbConnections.Oracle.Devart
{
    public class OracleDevartInternalDbConnectionCreator : IInternalDbConnectionCreator, ITransientDependency
    {
        public DatabaseProvider DatabaseProvider => DatabaseProvider.Oracle;

        public virtual IDbConnection Create(DbConnectionInfo dbConnectionInfo)
        {
            return new OracleConnection(dbConnectionInfo.ConnectionString);
        }
    }
}
