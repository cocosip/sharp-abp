using System.Collections.Generic;
using Volo.Abp.Collections;

namespace SharpAbp.Abp.Data.DbConnections
{
    public class AbpDataDbConnectionsOptions
    {
        public DbConnectionConfigurations DbConnections { get; }
        public HashSet<DatabaseProvider> DatabaseProviders { get; }
        public ITypeList<IDbConnectionCreator> Creators { get; }
        public AbpDataDbConnectionsOptions()
        {
            DbConnections = new DbConnectionConfigurations();
            DatabaseProviders = new HashSet<DatabaseProvider>();
            Creators = new TypeList<IDbConnectionCreator>();
        }
    }
}
