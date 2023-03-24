using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupPagedRequestDto : PagedAndSortedResultRequestDto
    {
        public string Name { get; set; }
    }
}
