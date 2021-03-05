using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Volo.Abp.AspNetCore.Mvc;

namespace SharpAbp.Abp.FileStoringManagement
{
    [Area("file-provider")]
    [Route("api/file-provider")]
    [Authorize(FileStoringPermissionConsts.FileStoringManagement)]
    public class FileProviderController : AbpController
    {
        private readonly IFileStoringAppService _fileStoringAppService;
        public FileProviderController(IFileStoringAppService fileStoringAppService)
        {
            _fileStoringAppService = fileStoringAppService;
        }

        [HttpGet]
        [Authorize(FileStoringPermissionConsts.ListProvider)]
        public List<ProviderDto> GetList()
        {
            return _fileStoringAppService.GetProviders();
        }

        [HttpGet]
        [Route("{provider}")]
        [Authorize(FileStoringPermissionConsts.GetProvierOptions)]
        public ProviderOptionsDto GetProviderOptions(string provider)
        {
            return _fileStoringAppService.GetProviderOptions(provider);
        }

    }
}
