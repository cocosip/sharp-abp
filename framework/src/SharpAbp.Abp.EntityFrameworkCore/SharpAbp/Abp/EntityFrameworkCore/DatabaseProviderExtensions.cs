using SharpAbp.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public static class DatabaseProviderExtensions
    {
        public static EfCoreDatabaseProvider? AsEfCoreDatabaseProvider(this DatabaseProvider databaseProvider)
        {
            return databaseProvider switch
            {
                DatabaseProvider.SqlServer => (EfCoreDatabaseProvider?)EfCoreDatabaseProvider.SqlServer,
                DatabaseProvider.MySql => (EfCoreDatabaseProvider?)EfCoreDatabaseProvider.MySql,
                DatabaseProvider.Oracle => (EfCoreDatabaseProvider?)EfCoreDatabaseProvider.Oracle,
                DatabaseProvider.PostgreSql => (EfCoreDatabaseProvider?)EfCoreDatabaseProvider.PostgreSql,
                DatabaseProvider.Sqlite => (EfCoreDatabaseProvider?)EfCoreDatabaseProvider.Sqlite,
                DatabaseProvider.InMemory => (EfCoreDatabaseProvider?)EfCoreDatabaseProvider.InMemory,
                DatabaseProvider.Cosmos => (EfCoreDatabaseProvider?)EfCoreDatabaseProvider.Cosmos,
                DatabaseProvider.Firebird => (EfCoreDatabaseProvider?)EfCoreDatabaseProvider.Firebird,
                _ => null,
            };
        }

        public static DatabaseProvider? AsDatabaseProvider(this EfCoreDatabaseProvider efCoreDatabaseProvider)
        {
            return efCoreDatabaseProvider switch
            {
                EfCoreDatabaseProvider.SqlServer => (DatabaseProvider?)DatabaseProvider.SqlServer,
                EfCoreDatabaseProvider.MySql => (DatabaseProvider?)DatabaseProvider.MySql,
                EfCoreDatabaseProvider.Oracle => (DatabaseProvider?)DatabaseProvider.Oracle,
                EfCoreDatabaseProvider.PostgreSql => (DatabaseProvider?)DatabaseProvider.PostgreSql,
                EfCoreDatabaseProvider.Sqlite => (DatabaseProvider?)DatabaseProvider.Sqlite,
                EfCoreDatabaseProvider.InMemory => (DatabaseProvider?)DatabaseProvider.InMemory,
                EfCoreDatabaseProvider.Cosmos => (DatabaseProvider?)DatabaseProvider.Cosmos,
                EfCoreDatabaseProvider.Firebird => (DatabaseProvider?)DatabaseProvider.Firebird,
                _ => null,
            };
        }
    }
}
