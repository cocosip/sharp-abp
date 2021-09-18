using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.Identity
{
    public class OrganizationUnitMemberPagedRequestDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
