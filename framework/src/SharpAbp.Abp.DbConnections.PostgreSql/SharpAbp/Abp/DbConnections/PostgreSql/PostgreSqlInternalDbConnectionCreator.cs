using Npgsql;
using SharpAbp.Abp.Data;
using System.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnections.PostgreSql
{
    [ExposeKeyedService<IInternalDbConnectionCreator>(DatabaseProvider.PostgreSql)]
    public class PostgreSqlInternalDbConnectionCreator : IInternalDbConnectionCreator, ITransientDependency
    {
        public DatabaseProvider DatabaseProvider => DatabaseProvider.PostgreSql;

        public virtual IDbConnection Create(DbConnectionInfo dbConnectionInfo)
        {
            return new NpgsqlConnection(dbConnectionInfo.ConnectionString);
        }
    }
}
