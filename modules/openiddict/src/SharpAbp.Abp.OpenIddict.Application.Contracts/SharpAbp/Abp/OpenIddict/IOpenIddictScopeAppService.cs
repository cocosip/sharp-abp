using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.OpenIddict
{
    public interface IOpenIddictScopeAppService : IApplicationService
    {
        Task<OpenIddictScopeDto> GetAsync(Guid id);
        Task<OpenIddictScopeDto> FindByNameAsync(string name);
        Task<PagedResultDto<OpenIddictScopeDto>> GetPagedListAsync(OpenIddictScopePagedRequestDto input);
        Task<List<OpenIddictScopeDto>> GetListAsync();
        Task<OpenIddictScopeDto> CreateAsync(CreateOpenIddictScopeDto input);
        Task<OpenIddictScopeDto> UpdateAsync(Guid id, UpdateOpenIddictScopeDto input);
        Task DeleteAsync(Guid id);
    }
}
