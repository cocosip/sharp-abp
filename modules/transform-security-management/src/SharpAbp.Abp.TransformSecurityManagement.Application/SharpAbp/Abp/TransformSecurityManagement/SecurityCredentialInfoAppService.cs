using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using SharpAbp.Abp.TransformSecurity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    [Authorize(TransformSecurityManagementPermissions.SecurityCredentialInfos.Default)]
    public class SecurityCredentialInfoAppService : TransformSecurityManagementAppServiceBase, ISecurityCredentialInfoAppService
    {
        protected ISecurityCredentialManager SecurityCredentialManager { get; }
        protected ISecurityCredentialInfoRepository SecurityCredentialInfoRepository { get; }
        public SecurityCredentialInfoAppService(
            ISecurityCredentialManager securityCredentialManager,
            ISecurityCredentialInfoRepository securityCredentialInfoRepository)
        {
            SecurityCredentialManager = securityCredentialManager;
            SecurityCredentialInfoRepository = securityCredentialInfoRepository;
        }

        public virtual async Task<SecurityCredentialInfoDto> GetAsync(Guid id)
        {
            var securityCredentialInfo = await SecurityCredentialInfoRepository.GetAsync(id);
            return ObjectMapper.Map<SecurityCredentialInfo, SecurityCredentialInfoDto>(securityCredentialInfo);
        }

        public virtual async Task<SecurityCredentialInfoDto> FindByIdentifierAsync([NotNull] string identifier)
        {
            Check.NotNullOrWhiteSpace(identifier, nameof(identifier));
            var securityCredentialInfo = await SecurityCredentialInfoRepository.FindByIdentifierAsync(identifier);
            return ObjectMapper.Map<SecurityCredentialInfo, SecurityCredentialInfoDto>(securityCredentialInfo);
        }

        public virtual async Task<List<SecurityCredentialInfoDto>> GetListAsync(
            string sorting = null,
            string identifier = "",
            string keyType = "",
            string bizType = "",
            DateTime? expiresMin = null,
            DateTime? expiresMax = null)
        {
            var securityCredentialInfos = await SecurityCredentialInfoRepository.GetListAsync(sorting, identifier, keyType, bizType, expiresMin, expiresMax);
            return ObjectMapper.Map<List<SecurityCredentialInfo>, List<SecurityCredentialInfoDto>>(securityCredentialInfos);
        }

        public virtual async Task<PagedResultDto<SecurityCredentialInfoDto>> GetPagedListAsync(
            SecurityCredentialInfoPagedRequestDto input)
        {
            var count = await SecurityCredentialInfoRepository.GetCountAsync(
                input.Identifier,
                input.KeyType,
                input.BizType,
                input.ExpiresMin,
                input.ExpiresMax);

            var securityCredentialInfos = await SecurityCredentialInfoRepository.GetPagedListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Identifier,
                input.KeyType,
                input.BizType,
                input.ExpiresMin,
                input.ExpiresMax);

            return new PagedResultDto<SecurityCredentialInfoDto>(
              count,
              ObjectMapper.Map<List<SecurityCredentialInfo>, List<SecurityCredentialInfoDto>>(securityCredentialInfos)
              );
        }

        [Authorize(TransformSecurityManagementPermissions.SecurityCredentialInfos.Create)]
        public virtual async Task<SecurityCredentialInfoDto> CreateAsync(CreateSecurityCredentialInfoDto input)
        {
            var securityCredential = await SecurityCredentialManager.GenerateAsync(input.BizType);
            var securityCredentialInfo = await SecurityCredentialInfoRepository.FindByIdentifierAsync(securityCredential.Identifier);
            return ObjectMapper.Map<SecurityCredentialInfo, SecurityCredentialInfoDto>(securityCredentialInfo);
        }

        [Authorize(TransformSecurityManagementPermissions.SecurityCredentialInfos.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await SecurityCredentialInfoRepository.DeleteAsync(id);
        }
    }
}
