using System;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class DatabaseConnectionInfoEtoMapper : IObjectMapper<DatabaseConnectionInfo, DatabaseConnectionInfoEto>, ITransientDependency
    {
        public DatabaseConnectionInfoEto Map(DatabaseConnectionInfo source)
        {
            if (source != null)
            {
                return new DatabaseConnectionInfoEto()
                {
                    Id = source.Id,
                    Name = source.Name,
                    DatabaseProvider = source.DatabaseProvider,
                    ConnectionString = source.ConnectionString
                };
            }

            return null;
        }

        public DatabaseConnectionInfoEto Map(
            DatabaseConnectionInfo source,
            DatabaseConnectionInfoEto destination)
        {
            if (destination == null)
            {
                destination = new DatabaseConnectionInfoEto();
            }
            if (source != null)
            {
                destination.Id = source.Id;
                if (!source.Name.IsNullOrWhiteSpace())
                {
                    destination.Name = source.Name;
                }

                if (!source.DatabaseProvider.IsNullOrWhiteSpace())
                {
                    destination.DatabaseProvider = source.DatabaseProvider;
                }

                if (!source.ConnectionString.IsNullOrWhiteSpace())
                {
                    destination.ConnectionString = source.ConnectionString;
                }

            }
            return destination;
        }
    }
}
