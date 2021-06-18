using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    [RemoteService(Name = DbConnectionsManagementRemoteServiceConsts.RemoteServiceName)]
    [Area("dbconnections")]
    [Route("api/database-provider")]
    public class DatabaseProviderController : DbConnectionsController, IDatabaseProviderAppService
    {
        private readonly IDatabaseProviderAppService _databaseProviderAppService;
        public DatabaseProviderController(IDatabaseProviderAppService databaseProviderAppService)
        {
            _databaseProviderAppService = databaseProviderAppService;
        }

        [HttpGet]
        [Route("all")]
        public async Task<List<string>> GetAllAsync()
        {
            return await _databaseProviderAppService.GetAllAsync();
        }
    }
}
