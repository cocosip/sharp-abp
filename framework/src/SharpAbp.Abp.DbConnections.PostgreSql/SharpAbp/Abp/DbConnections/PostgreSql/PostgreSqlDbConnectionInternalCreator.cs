using Npgsql;
using System.Data;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnections.PostgreSql
{
    public class PostgreSqlDbConnectionInternalCreator : IDbConnectionInternalCreator, ITransientDependency
    {
        public DatabaseProvider DatabaseProvider => DatabaseProvider.PostgreSql;

        public virtual IDbConnection Create(DbConnectionInfo dbConnectionInfo)
        {
            Check.NotNullOrWhiteSpace(dbConnectionInfo.ConnectionString, nameof(dbConnectionInfo.ConnectionString));

            return new NpgsqlConnection(dbConnectionInfo.ConnectionString);
        }
    }
}
