using SharpAbp.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public static class DatabaseProviderExtensions
    {
        public static EfCoreDatabaseProvider? AsEfCoreDatabaseProvider(this DatabaseProvider databaseProvider)
        {
            switch (databaseProvider)
            {
                case DatabaseProvider.SqlServer:
                    return EfCoreDatabaseProvider.SqlServer;
                case DatabaseProvider.MySql:
                    return EfCoreDatabaseProvider.MySql;
                case DatabaseProvider.Oracle:
                    return EfCoreDatabaseProvider.Oracle;
                case DatabaseProvider.PostgreSql:
                    return EfCoreDatabaseProvider.PostgreSql;
                case DatabaseProvider.Sqlite:
                    return EfCoreDatabaseProvider.Sqlite;
                case DatabaseProvider.InMemory:
                    return EfCoreDatabaseProvider.InMemory;
                case DatabaseProvider.Cosmos:
                    return EfCoreDatabaseProvider.Cosmos;
                case DatabaseProvider.Firebird:
                    return EfCoreDatabaseProvider.Firebird;
                default:
                    return null;
            }
        }

        public static DatabaseProvider? AsDatabaseProvider(this EfCoreDatabaseProvider efCoreDatabaseProvider)
        {
            switch (efCoreDatabaseProvider)
            {
                case EfCoreDatabaseProvider.SqlServer:
                    return DatabaseProvider.SqlServer;
                case EfCoreDatabaseProvider.MySql:
                    return DatabaseProvider.MySql;
                case EfCoreDatabaseProvider.Oracle:
                    return DatabaseProvider.Oracle;
                case EfCoreDatabaseProvider.PostgreSql:
                    return DatabaseProvider.PostgreSql;
                case EfCoreDatabaseProvider.Sqlite:
                    return DatabaseProvider.Sqlite;
                case EfCoreDatabaseProvider.InMemory:
                    return DatabaseProvider.InMemory;
                case EfCoreDatabaseProvider.Cosmos:
                    return DatabaseProvider.Cosmos;
                case EfCoreDatabaseProvider.Firebird:
                    return DatabaseProvider.Firebird;
                default:
                    return null;
            }
        }
    }
}
