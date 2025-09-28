using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    /// <summary>
    /// Application service interface for managing database providers.
    /// Provides operations for retrieving available database provider information.
    /// </summary>
    public interface IDatabaseProviderAppService : IApplicationService
    {
        /// <summary>
        /// Retrieves all available database providers supported by the system.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of database provider names.</returns>
        Task<List<string>> GetAllAsync();
    }
}
