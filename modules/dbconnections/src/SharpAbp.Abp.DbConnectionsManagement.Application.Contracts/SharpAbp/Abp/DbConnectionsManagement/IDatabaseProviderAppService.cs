using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public interface IDatabaseProviderAppService : IApplicationService
    {
        /// <summary>
        /// Get all database provider
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GetAllAsync();
    }
}
