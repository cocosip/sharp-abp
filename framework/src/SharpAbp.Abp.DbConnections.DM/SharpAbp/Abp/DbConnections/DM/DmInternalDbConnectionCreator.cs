using Dm;
using SharpAbp.Abp.Data;
using System.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnections.DM
{
    public class DmInternalDbConnectionCreator : IInternalDbConnectionCreator, ITransientDependency
    {
        public DatabaseProvider DatabaseProvider => DatabaseProvider.MySql;

        public virtual IDbConnection Create(DbConnectionInfo dbConnectionInfo)
        {
            return new DmConnection(dbConnectionInfo.ConnectionString);
        }
    }
}
