using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.DbConnections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    [Authorize(DbConnectionsManagementPermissions.DatabaseConnectionInfos.Default)]
    public class DatabaseProviderAppService : DbConnectionsManagementAppServiceBase, IDatabaseProviderAppService
    {
        protected AbpDbConnectionsOptions Options { get; }
        public DatabaseProviderAppService(IOptions<AbpDbConnectionsOptions> options)
        {
            Options = options.Value;
        }

        /// <summary>
        /// Get all database provider
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<string>> GetAllAsync()
        {
            var databaseProviders = Options.DatabaseProviders.Select(x => x.ToString()).ToList();
            return await Task.FromResult(databaseProviders);
        }
    }
}
