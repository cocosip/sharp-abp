using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.IdentityServer.ApiScopes
{
    public class ApiScopePagedRequestDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
