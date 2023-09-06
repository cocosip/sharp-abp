﻿using Microsoft.Data.SqlClient;
using SharpAbp.Abp.Data;
using System.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnections.SqlServer
{
    public class SqlServerInternalDbConnectionCreator : IInternalDbConnectionCreator, ITransientDependency
    {
        public DatabaseProvider DatabaseProvider => DatabaseProvider.SqlServer;

        public virtual IDbConnection Create(DbConnectionInfo dbConnectionInfo)
        {
            return new SqlConnection(dbConnectionInfo.ConnectionString);
        }
    }
}
