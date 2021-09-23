using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.FileStoringManagement
{
    [RemoteService(Name = FileStoringManagementRemoteServiceConsts.RemoteServiceName)]
    [Area("file-storing")]
    [Route("api/file-storing/containers")]
    public class ContainerController : FileStoringController, IContainerAppService
    {
        private readonly IContainerAppService _containerAppService;
        public ContainerController(IContainerAppService containerAppService)
        {
            _containerAppService = containerAppService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ContainerDto> GetAsync(Guid id)
        {
            return await _containerAppService.GetAsync(id);
        }

        [HttpGet]
        public async Task<PagedResultDto<ContainerDto>> GetPagedListAsync(FileStoringContainerPagedRequestDto input)
        {
            return await _containerAppService.GetPagedListAsync(input);
        }

        [HttpGet]
        [Route("find-by-name/{name}")]
        public async Task<ContainerDto> FindByNameAsync(string name)
        {
            return await _containerAppService.FindByNameAsync(name);
        }

        [HttpPost]
        public async Task<Guid> CreateAsync(CreateContainerDto input)
        {
            return await _containerAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task UpdateAsync(Guid id, UpdateContainerDto input)
        {
            await _containerAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _containerAppService.DeleteAsync(id);
        }
    }
}
