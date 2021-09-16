using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.IdentityServer.IdentityResources
{
    public class IdentityResourcePagedRequestDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
