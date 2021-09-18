using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.Identity
{
    public class OrganizationUnitUnaddedMemberPagedRequestDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
