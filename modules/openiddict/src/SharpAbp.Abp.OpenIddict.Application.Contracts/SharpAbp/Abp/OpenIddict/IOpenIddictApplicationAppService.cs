using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.OpenIddict
{
    public interface IOpenIddictApplicationAppService : IApplicationService
    {
        Task<OpenIddictApplicationDto> GetAsync(Guid id);
        Task<OpenIddictApplicationDto> FindByClientIdAsync(string clientId);
        Task<PagedResultDto<OpenIddictApplicationDto>> GetPagedListAsync(PagedAndSortedResultRequestDto input);
        Task<List<OpenIddictApplicationDto>> GetListAsync();
        Task<OpenIddictApplicationDto> CreateAsync(CreateOrUpdateOpenIddictApplicationDto input);
        Task<OpenIddictApplicationDto> UpdateAsync(Guid id, CreateOrUpdateOpenIddictApplicationDto input);
        Task DeleteAsync(Guid id);
    }
}
