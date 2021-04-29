using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.FileStoringManagement
{
    [RemoteService(Name = FileStoringManagementRemoteServiceConsts.RemoteServiceName)]
    [Area("file-storing")]
    [Route("api/container")]
    public class ContainerController : FileStoringController, IContainerAppService
    {
        private readonly IContainerAppService _containerAppService;
        public ContainerController(IContainerAppService containerAppService)
        {
            _containerAppService = containerAppService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ContainerDto> GetAsync(
            Guid id,
            bool includeDetails = true)
        {
            return await _containerAppService.GetAsync(id, includeDetails);
        }

        [HttpGet]
        public async Task<PagedResultDto<ContainerDto>> GetPagedListAsync(
            FileStoringContainerPagedRequestDto input, 
            bool includeDetails = true)
        {
            return await _containerAppService.GetPagedListAsync(input, includeDetails);
        }

        [HttpGet]
        [Route("find-by-name/{name}")]
        public async Task<ContainerDto> FindByNameAsync(
            string name, 
            bool includeDetails = true)
        {
            return await _containerAppService.FindByNameAsync(name, includeDetails);
        }

        [HttpPost]
        public async Task<Guid> CreateAsync(CreateContainerDto input)
        {
            return await _containerAppService.CreateAsync(input);
        }

        [HttpPut]
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
