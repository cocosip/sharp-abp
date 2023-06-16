using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public interface ITenantGroupAppService : IApplicationService
    {
        Task<TenantGroupDto> GetAsync(Guid id);
        Task<TenantGroupDto> FindByNameAsync(string name);
        Task<PagedResultDto<TenantGroupDto>> GetPagedListAsync(TenantGroupPagedRequestDto input);
        Task<List<TenantGroupDto>> GetListAsync(string sorting = null, string name = "");
        Task<List<TenantDto>> GetAvialableTenantsAsync();
        Task<TenantGroupDto> CreateAsync(CreateTenantGroupDto input);
        Task<TenantGroupDto> UpdateAsync(Guid id, UpdateTenantGroupDto input);
        Task DeleteAsync(Guid id);
        Task<TenantGroupDto> AddTenantAsync(Guid id, AddTenantDto input);
        Task<TenantGroupDto> RemoveTenantAsync(Guid id, Guid tenantGroupTenantId);
        Task<string> GetDefaultConnectionStringAsync(Guid id);
        Task UpdateDefaultConnectionStringAsync(Guid id, string defaultConnectionString);
        Task DeleteDefaultConnectionStringAsync(Guid id);
    }
}
