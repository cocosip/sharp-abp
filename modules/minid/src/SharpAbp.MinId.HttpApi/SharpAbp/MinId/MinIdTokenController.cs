using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.MinId
{
    [RemoteService(Name = MinIdRemoteServiceConsts.RemoteServiceName)]
    [Area("minid")]
    [Route("api/minid-token")]
    public class MinIdTokenController : MinIdController, IMinIdTokenAppService
    {
        private readonly IMinIdTokenAppService _minIdTokenAppService;
        public MinIdTokenController(IMinIdTokenAppService minIdTokenAppService)
        {
            _minIdTokenAppService = minIdTokenAppService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<MinIdTokenDto> GetAsync(Guid id)
        {
            return await _minIdTokenAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("find-by-token/{bizType}/{token}")]
        public async Task<MinIdTokenDto> FindByTokenAsync(string bizType, string token)
        {
            return await _minIdTokenAppService.FindByTokenAsync(bizType, token);
        }

        [HttpGet]
        public async Task<PagedResultDto<MinIdTokenDto>> GetPagedListAsync(MinIdTokenPagedRequestDto input)
        {
            return await _minIdTokenAppService.GetPagedListAsync(input);
        }

        [HttpPost]
        public async Task<Guid> CreateAsync(CreateMinIdTokenDto input)
        {
            return await _minIdTokenAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task UpdateAsync(Guid id, UpdateMinIdTokenDto input)
        {
            await _minIdTokenAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _minIdTokenAppService.DeleteAsync(id);
        }
    }
}
