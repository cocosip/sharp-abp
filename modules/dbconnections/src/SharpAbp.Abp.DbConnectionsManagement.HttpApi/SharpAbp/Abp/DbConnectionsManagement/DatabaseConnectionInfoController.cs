using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    [RemoteService(Name = DbConnectionsManagementRemoteServiceConsts.RemoteServiceName)]
    [Area("dbconnections")]
    [Route("api/dbconnections/database-connectioninfos")]
    public class DatabaseConnectionInfoController : DbConnectionsController, IDatabaseConnectionInfoAppService
    {
        private readonly IDatabaseConnectionInfoAppService _databaseConnectionInfoAppService;

        public DatabaseConnectionInfoController(IDatabaseConnectionInfoAppService databaseConnectionInfoAppService)
        {
            _databaseConnectionInfoAppService = databaseConnectionInfoAppService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<DatabaseConnectionInfoDto> GetAsync(Guid id)
        {
            return await _databaseConnectionInfoAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("find-by-name/{name}")]
        public async Task<DatabaseConnectionInfoDto> FindByNameAsync(string name)
        {
            return await _databaseConnectionInfoAppService.FindByNameAsync(name);
        }

        [HttpGet]
        public async Task<PagedResultDto<DatabaseConnectionInfoDto>> GetPagedListAsync(DatabaseConnectionInfoPagedRequestDto input)
        {
            return await _databaseConnectionInfoAppService.GetPagedListAsync(input);
        }

        [HttpPost]
        public async Task<Guid> CreateAsync(CreateDatabaseConnectionInfoDto input)
        {
            return await _databaseConnectionInfoAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task UpdateAsync(Guid id, UpdateDatabaseConnectionInfoDto input)
        {
            await _databaseConnectionInfoAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _databaseConnectionInfoAppService.DeleteAsync(id);
        }

    }
}
