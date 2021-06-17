using Volo.Abp.Collections;

namespace SharpAbp.Abp.DbConnections
{
    public class DatabaseProviderConfiguration
    {
        public DatabaseProvider DatabaseProvider { get; set; }

        public ITypeList<IDbConnectionCreator> Creators { get; set; }

        public DatabaseProviderConfiguration()
        {
            Creators = new TypeList<IDbConnectionCreator>();
        }

        public DatabaseProviderConfiguration(DatabaseProvider databaseProvider)
        {
            DatabaseProvider = databaseProvider;
        }
    }
}
