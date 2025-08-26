using System.Data;
using GaussDB;
using SharpAbp.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnections.GaussDB
{
    [ExposeKeyedService<IInternalDbConnectionCreator>(DatabaseProvider.GaussDB)]
    public class GaussDBInternalDbConnectionCreator : IInternalDbConnectionCreator, ITransientDependency
    {
        public DatabaseProvider DatabaseProvider => DatabaseProvider.GaussDB;

        public virtual IDbConnection Create(DbConnectionInfo dbConnectionInfo)
        {
            return new GaussDBConnection(dbConnectionInfo.ConnectionString);
        }
    }
}
