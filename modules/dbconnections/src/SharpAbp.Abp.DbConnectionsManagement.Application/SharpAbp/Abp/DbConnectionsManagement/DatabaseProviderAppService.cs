using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.DbConnections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    /// <summary>
    /// Application service implementation for managing database providers.
    /// Provides functionality to retrieve available database provider information from configuration.
    /// </summary>
    [Authorize(DbConnectionsManagementPermissions.DatabaseConnectionInfos.Default)]
    public class DatabaseProviderAppService : DbConnectionsManagementAppServiceBase, IDatabaseProviderAppService
    {
        /// <summary>
        /// Gets the database connections configuration options.
        /// </summary>
        protected AbpDbConnectionsOptions Options { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseProviderAppService"/> class.
        /// </summary>
        /// <param name="options">The database connections configuration options.</param>
        public DatabaseProviderAppService(IOptions<AbpDbConnectionsOptions> options)
        {
            Options = options.Value;
        }

        /// <summary>
        /// Retrieves all available database providers supported by the system.
        /// The providers are obtained from the application configuration settings.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of database provider names.</returns>
        public virtual async Task<List<string>> GetAllAsync()
        {
            var databaseProviders = Options.DatabaseProviders.Select(x => x.ToString()).ToList();
            return await Task.FromResult(databaseProviders);
        }
    }
}
