using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace SharpAbp.Abp.FileStoringManagement
{
    //[RemoteService(Name = FileStoringRemoteServiceConsts.RemoteServiceName)]
    [Area("container")]
    [Route("api/container")]
    [Authorize(FileStoringPermissionConsts.FileStoringManagement)]
    public class ContainerController : AbpController
    {
        private readonly IFileStoringAppService _fileStoringAppService;
        public ContainerController(IFileStoringAppService fileStoringAppService)
        {
            _fileStoringAppService = fileStoringAppService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ContainerDto> GetAsync(Guid id)
        {
            return await _fileStoringAppService.GetAsync(id, true);
        }

        [HttpGet]
        [Authorize(FileStoringPermissionConsts.ListContainer)]
        public async Task<PagedResultDto<ContainerDto>> GetPagedListAsync(FileStoringContainerPagedRequestDto input)
        {
            return await _fileStoringAppService.GetPagedListAsync(input, true);
        }

        [HttpPost]
        [Authorize(FileStoringPermissionConsts.CreateContainer)]
        public async Task<Guid> CreateAsync(CreateContainerDto input)
        {
            return await _fileStoringAppService.CreateAsync(input);
        }

        [HttpPut]
        [Authorize(FileStoringPermissionConsts.UpdateContainer)]
        public async Task UpdateAsync(UpdateContainerDto input)
        {
            await _fileStoringAppService.UpdateAsync(input);
        }

        [HttpPatch]
        [Route("{id}")]
        [Authorize(FileStoringPermissionConsts.DeleteContainer)]
        public async Task DeleteAsync(Guid id)
        {
            await _fileStoringAppService.DeleteAsync(id);
        }

    }
}
