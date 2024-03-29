﻿using System.Data;
using System.Threading.Tasks;

namespace SharpAbp.Abp.DbConnections
{
    public interface IDbConnectionCreator
    {
        bool IsMatch(string dbConnectionName, DbConnectionInfo dbConnectionInfo);

        Task<IDbConnection> CreateAsync(string dbConnectionName, DbConnectionInfo dbConnectionInfo);
    }
}
