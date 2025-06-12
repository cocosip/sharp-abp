using System.Data;
using Dm;
using SharpAbp.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnections.DM
{
    [ExposeKeyedService<IInternalDbConnectionCreator>(DatabaseProvider.Dm)]
    public class DmInternalDbConnectionCreator : IInternalDbConnectionCreator, ITransientDependency
    {
        public DatabaseProvider DatabaseProvider => DatabaseProvider.Dm;

        public virtual IDbConnection Create(DbConnectionInfo dbConnectionInfo)
        {
            return new DmConnection(dbConnectionInfo.ConnectionString);
        }
    }
}
