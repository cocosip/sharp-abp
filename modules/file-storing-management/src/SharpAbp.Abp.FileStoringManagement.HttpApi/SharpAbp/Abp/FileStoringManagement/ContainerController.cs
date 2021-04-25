using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.FileStoringManagement
{
    //[RemoteService(Name = FileStoringRemoteServiceConsts.RemoteServiceName)]
    [Area("file-storing")]
    [Route("api/container")]
    public class ContainerController : FileStoringController
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
            return await _containerAppService.GetAsync(id, true);
        }

        [HttpGet]
        [Route("find-by-name/{name}")]
        public virtual async Task<ContainerDto> FindByNameAsync(string name)
        {
            return await _containerAppService.FindByNameAsync(name, true);
        }


        [HttpGet]
        public async Task<PagedResultDto<ContainerDto>> GetPagedListAsync(FileStoringContainerPagedRequestDto input)
        {
            return await _containerAppService.GetPagedListAsync(input, true);
        }

        [HttpPost]
        public async Task<Guid> CreateAsync([FromBody] CreateContainerDto input)
        {
            return await _containerAppService.CreateAsync(input);
        }

        [HttpPut]
        public async Task UpdateAsync([FromBody] UpdateContainerDto input)
        {
            await _containerAppService.UpdateAsync(input);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _containerAppService.DeleteAsync(id);
        }

    }
}
