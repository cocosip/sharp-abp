using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public class EfCoreDatabaseProviderAccessor : IEfCoreDatabaseProviderAccessor, ITransientDependency
    {
        /// <summary>
        /// Get databaseProvider or null
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public virtual EfCoreDatabaseProvider? GetDatabaseProviderOrNull(string providerName)
        {
            switch (providerName)
            {
                case "Microsoft.EntityFrameworkCore.SqlServer":
                    return EfCoreDatabaseProvider.SqlServer;
                case "Npgsql.EntityFrameworkCore.PostgreSQL":
                    return EfCoreDatabaseProvider.PostgreSql;
                case "Pomelo.EntityFrameworkCore.MySql":
                    return EfCoreDatabaseProvider.MySql;
                case "Oracle.EntityFrameworkCore":
                case "Devart.Data.Oracle.Entity.EFCore":
                    return EfCoreDatabaseProvider.Oracle;
                case "Microsoft.EntityFrameworkCore.Sqlite":
                    return EfCoreDatabaseProvider.Sqlite;
                case "Microsoft.EntityFrameworkCore.InMemory":
                    return EfCoreDatabaseProvider.InMemory;
                case "FirebirdSql.EntityFrameworkCore.Firebird":
                    return EfCoreDatabaseProvider.Firebird;
                case "Microsoft.EntityFrameworkCore.Cosmos":
                    return EfCoreDatabaseProvider.Cosmos;
                default:
                    return null;
            }
        }
    }
}
