using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.OpenIddict
{
    public class OpenIddictScopePagedRequestDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
