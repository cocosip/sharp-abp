using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.CryptoVault
{
    [RemoteService(Name = AbpCryptoVaultRemoteServiceConsts.RemoteServiceName)]
    [Area("cryptoVault")]
    [Route("api/crypto-vault/sm2Creds")]
    public class SM2CredsController : CryptoVaultController, ISM2CredsAppService
    {
        private readonly ISM2CredsAppService _sm2CredsAppService;
        public SM2CredsController(ISM2CredsAppService sm2CredsAppService)
        {
            _sm2CredsAppService = sm2CredsAppService;
        }

        [HttpGet]
        [Route("find-by-identifier/{identifier}")]
        public async Task<SM2CredsDto> FindByIdentifierAsync([NotNull] string identifier)
        {
            return await _sm2CredsAppService.FindByIdentifierAsync(identifier);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<SM2CredsDto> GetAsync(Guid id)
        {
            return await _sm2CredsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("get-list")]
        public async Task<List<SM2CredsDto>> GetListAsync(string sorting = null, string identifier = "", int? sourceType = null, string curve = "")
        {
            return await _sm2CredsAppService.GetListAsync(sorting, identifier, sourceType, curve);
        }

        [HttpGet]
        public async Task<PagedResultDto<SM2CredsDto>> GetPagedListAsync(SM2CredsPagedRequestDto input)
        {
            return await _sm2CredsAppService.GetPagedListAsync(input);
        }

        [HttpPost]
        public async Task<SM2CredsDto> CreateAsync(CreateSM2CredsDto input)
        {
            return await _sm2CredsAppService.CreateAsync(input);
        }

        [HttpPost]
        [Route("generate")]
        public async Task GenerateAsync(GenerateSM2CredsDto input)
        {
            await _sm2CredsAppService.GenerateAsync(input);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _sm2CredsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("decrypt-key/{id}")]
        public async Task<SM2CredsKeyDto> GetDecryptKey(Guid id)
        {
            return await _sm2CredsAppService.GetDecryptKey(id);
        }
    }
}
