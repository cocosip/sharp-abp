using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.MinId
{
    [RemoteService(Name = MinIdRemoteServiceConsts.RemoteServiceName)]
    [Area("minid")]
    [Route("api/minid/minid-info")]
    public class MinIdInfoController : MinIdController, IMinIdInfoAppService
    {
        private readonly IMinIdInfoAppService _minIdInfoAppService;
        public MinIdInfoController(IMinIdInfoAppService minIdInfoAppService)
        {
            _minIdInfoAppService = minIdInfoAppService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<MinIdInfoDto> GetAsync(Guid id)
        {
            return await _minIdInfoAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("find-by-bizType/{bizType}")]
        public async Task<MinIdInfoDto> FindByBizTypeAsync(string bizType)
        {
            return await _minIdInfoAppService.FindByBizTypeAsync(bizType);
        }

        [HttpGet]
        public async Task<PagedResultDto<MinIdInfoDto>> GetPagedListAsync(MinIdInfoPagedRequestDto input)
        {
            return await _minIdInfoAppService.GetPagedListAsync(input);
        }

        [HttpGet]
        [Route("get-list")]
        public async Task<List<MinIdInfoDto>> GetListAsync(string sorting = null, string bizType = "")
        {
            return await _minIdInfoAppService.GetListAsync(sorting, bizType);
        }

        [HttpPost]
        public async Task<MinIdInfoDto> CreateAsync(CreateMinIdInfoDto input)
        {
            return await _minIdInfoAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<MinIdInfoDto> UpdateAsync(Guid id, UpdateMinIdInfoDto input)
        {
            return await _minIdInfoAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _minIdInfoAppService.DeleteAsync(id);
        }
    }
}
