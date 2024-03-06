using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    public interface ISecurityCredentialInfoAppService : IApplicationService
    {
        Task<SecurityCredentialInfoDto> GetAsync(Guid id);
        Task<SecurityCredentialInfoDto> FindByIdentifierAsync(string identifier);
        Task<List<SecurityCredentialInfoDto>> GetListAsync(string sorting = null, string identifier = "", string keyType = "", string bizType = "", DateTime? expiresMin = null, DateTime? expiresMax = null);
        Task<PagedResultDto<SecurityCredentialInfoDto>> GetPagedListAsync(SecurityCredentialInfoPagedRequestDto input);
        Task<SecurityCredentialInfoDto> CreateAsync(CreateSecurityCredentialInfoDto input);
        Task DeleteAsync(Guid id);
    }

}
