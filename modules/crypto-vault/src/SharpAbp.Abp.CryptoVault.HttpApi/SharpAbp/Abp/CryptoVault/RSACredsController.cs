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
    [Route("api/crypto-vault/rsaCreds")]
    public class RSACredsController : CryptoVaultController, IRSACredsAppService
    {
        private readonly IRSACredsAppService _rsaCredsAppService;
        public RSACredsController(IRSACredsAppService rsaCredsAppService)
        {
            _rsaCredsAppService = rsaCredsAppService;
        }

        [HttpGet]
        [Route("find-by-identifier/{identifier}")]
        public async Task<RSACredsDto> FindByIdentifierAsync(string identifier)
        {
            return await _rsaCredsAppService.FindByIdentifierAsync(identifier);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<RSACredsDto> GetAsync(Guid id)
        {
            return await _rsaCredsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("get-list")]
        public async Task<List<RSACredsDto>> GetListAsync(string sorting = null, string identifier = "", int? sourceType = null, int? size = null)
        {
            return await _rsaCredsAppService.GetListAsync(sorting, identifier, sourceType, size);
        }

        [HttpGet]
        public async Task<PagedResultDto<RSACredsDto>> GetPagedListAsync(RSACredsPagedRequestDto input)
        {
            return await _rsaCredsAppService.GetPagedListAsync(input);
        }

        [HttpPost]
        public async Task<RSACredsDto> CreateAsync(CreateRSACredsDto input)
        {
            return await _rsaCredsAppService.CreateAsync(input);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _rsaCredsAppService.DeleteAsync(id);
        }

        [HttpPost]
        [Route("generate")]
        public async Task GenerateAsync(GenerateRSACredsDto input)
        {
            await _rsaCredsAppService.GenerateAsync(input);
        }

        [HttpGet]
        [Route("decrypt-key/{id}")]
        public async Task<RSACredsKeyDto> GetDecryptKey(Guid id)
        {
            return await _rsaCredsAppService.GetDecryptKey(id);
        }
    }
}
