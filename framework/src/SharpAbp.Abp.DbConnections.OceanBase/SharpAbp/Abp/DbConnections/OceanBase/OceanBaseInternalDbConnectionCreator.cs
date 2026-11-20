using System.Data;
using OceanBaseConnector;
using SharpAbp.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp;

namespace SharpAbp.Abp.DbConnections.OceanBase
{
    [ExposeKeyedService<IInternalDbConnectionCreator>(DatabaseProvider.OceanBase)]
    public class OceanBaseInternalDbConnectionCreator : IInternalDbConnectionCreator, ITransientDependency
    {
        public DatabaseProvider DatabaseProvider => DatabaseProvider.OceanBase;

        public virtual IDbConnection Create(DbConnectionInfo dbConnectionInfo)
        {
            Check.NotNull(dbConnectionInfo, nameof(dbConnectionInfo));
            return new OceanBaseConnection(Check.NotNullOrWhiteSpace(dbConnectionInfo.ConnectionString,
                nameof(dbConnectionInfo.ConnectionString)));
        }
    }
}
