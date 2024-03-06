using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    [RemoteService(Name = AbpTransformSecurityManagementRemoteServiceConsts.RemoteServiceName)]
    [Area("cryptoVault")]
    [Route("api/transform-security-management/security-credentialInfos")]
    public class SecurityCredentialInfoController : TransformSecurityManagementController, ISecurityCredentialInfoAppService
    {
        private readonly ISecurityCredentialInfoAppService _securityCredentialInfoAppService;
        public SecurityCredentialInfoController(ISecurityCredentialInfoAppService securityCredentialInfoAppService)
        {
            _securityCredentialInfoAppService = securityCredentialInfoAppService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<SecurityCredentialInfoDto> GetAsync(Guid id)
        {
            return await _securityCredentialInfoAppService.GetAsync(id);
        }

        [HttpGet]
        public async Task<PagedResultDto<SecurityCredentialInfoDto>> GetPagedListAsync(SecurityCredentialInfoPagedRequestDto input)
        {
            return await _securityCredentialInfoAppService.GetPagedListAsync(input);
        }

        [HttpGet]
        [Route("get-list")]
        public async Task<List<SecurityCredentialInfoDto>> GetListAsync(string sorting = null, string identifier = "", string keyType = "", string bizType = "", DateTime? expiresMin = null, DateTime? expiresMax = null)
        {
            return await _securityCredentialInfoAppService.GetListAsync(sorting, identifier, keyType, bizType, expiresMin, expiresMax);
        }

        [HttpGet]
        [Route("find-by-identifier/{identifier}")]
        public async Task<SecurityCredentialInfoDto> FindByIdentifierAsync(string identifier)
        {
            return await _securityCredentialInfoAppService.FindByIdentifierAsync(identifier);
        }

        [HttpPost]
        public async Task<SecurityCredentialInfoDto> CreateAsync(CreateSecurityCredentialInfoDto input)
        {
            return await _securityCredentialInfoAppService.CreateAsync(input);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _securityCredentialInfoAppService.DeleteAsync(id);
        }

 
    }
}
