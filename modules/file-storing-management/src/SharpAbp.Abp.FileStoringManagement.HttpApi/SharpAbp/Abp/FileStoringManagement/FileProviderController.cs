using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoringManagement
{
    [RemoteService(Name = FileStoringManagementRemoteServiceConsts.RemoteServiceName)]
    [Area("file-storing")]
    [Route("api/file-storing/file-providers")]
    public class FileProviderController : FileStoringController, IFileProviderAppService
    {
        private readonly IFileProviderAppService _fileProviderAppService;
        public FileProviderController(IFileProviderAppService fileProviderAppService)
        {
            _fileProviderAppService = fileProviderAppService;
        }

        [HttpGet]
        public async Task<List<ProviderDto>> GetProvidersAsync()
        {
            return await _fileProviderAppService.GetProvidersAsync();
        }

        [HttpGet]
        [Route("has-provider/{provider}")]
        public async Task<bool> HasProviderAsync(string provider)
        {
            return await _fileProviderAppService.HasProviderAsync(provider);
        }

        [HttpGet]
        [Route("get-options/{provider}")]
        public async Task<ProviderOptionsDto> GetOptionsAsync(string provider)
        {
            return await _fileProviderAppService.GetOptionsAsync(provider);
        }

    }
}
