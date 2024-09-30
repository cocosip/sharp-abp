using SharpAbp.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public class EfCoreDatabaseProviderAccessor : IEfCoreDatabaseProviderAccessor, ITransientDependency
    {
        /// <summary>
        /// Get databaseProvider or null
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public virtual DatabaseProvider? GetDatabaseProviderOrNull(string providerName)
        {
            switch (providerName)
            {
                case "Microsoft.EntityFrameworkCore.SqlServer":
                    return DatabaseProvider.SqlServer;
                case "Npgsql.EntityFrameworkCore.PostgreSQL":
                    return DatabaseProvider.PostgreSql;
                case "Pomelo.EntityFrameworkCore.MySql":
                    return DatabaseProvider.MySql;
                case "Oracle.EntityFrameworkCore":
                case "Devart.Data.Oracle.Entity.EFCore":
                    return DatabaseProvider.Oracle;
                case "Microsoft.EntityFrameworkCore.Sqlite":
                    return DatabaseProvider.Sqlite;
                case "Microsoft.EntityFrameworkCore.InMemory":
                    return DatabaseProvider.InMemory;
                case "FirebirdSql.EntityFrameworkCore.Firebird":
                    return DatabaseProvider.Firebird;
                case "Microsoft.EntityFrameworkCore.Cosmos":
                    return DatabaseProvider.Cosmos;
                case "DM.Microsoft.EntityFrameworkCore":
                    return DatabaseProvider.Dm;
                default:
                    return null;
            }
        }
    }
}
