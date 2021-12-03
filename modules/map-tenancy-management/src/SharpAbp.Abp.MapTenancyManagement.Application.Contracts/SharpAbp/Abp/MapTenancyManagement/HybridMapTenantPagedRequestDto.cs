using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class HybridMapTenantPagedRequestDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}