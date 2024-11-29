using Devart.Data.Oracle;
using SharpAbp.Abp.Data;
using System.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnections.Oracle.Drvart
{
    [ExposeKeyedService<IInternalDbConnectionCreator>(DatabaseProvider.Oracle)]
    public class OracleDevartInternalDbConnectionCreator : IInternalDbConnectionCreator, ITransientDependency
    {
        public DatabaseProvider DatabaseProvider => DatabaseProvider.Oracle;

        public virtual IDbConnection Create(DbConnectionInfo dbConnectionInfo)
        {
            return new OracleConnection(dbConnectionInfo.ConnectionString);
        }
    }
}
