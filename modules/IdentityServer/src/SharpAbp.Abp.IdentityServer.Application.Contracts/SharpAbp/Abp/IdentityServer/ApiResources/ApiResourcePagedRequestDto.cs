using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.IdentityServer.ApiResources
{
    public class ApiResourcePagedRequestDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}