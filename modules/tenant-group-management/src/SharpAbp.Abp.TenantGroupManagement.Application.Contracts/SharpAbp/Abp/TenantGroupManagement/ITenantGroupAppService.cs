using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public interface ITenantGroupAppService : IApplicationService
    {
        Task<TenantGroupDto> GetAsync(Guid id);
        Task<TenantGroupDto> FindByNameAsync(string name);
        Task<PagedResultDto<TenantGroupDto>> GetPagedListAsync(TenantGroupPagedRequestDto input);
        Task<List<TenantGroupDto>> GetListAsync(string sorting = null, string name = "");
        Task<TenantGroupDto> CreateAsync(CreateTenantGroupDto input);
        Task<TenantGroupDto> UpdateAsync(Guid id, CreateTenantGroupDto input);
        Task DeleteAsync(Guid id);
        Task<TenantGroupDto> AddTenantAsync(Guid id, AddTenantDto input);
        Task<TenantGroupDto> RemoveTenantAsync(Guid id, Guid tenantGroupTenantId);
    }
}
