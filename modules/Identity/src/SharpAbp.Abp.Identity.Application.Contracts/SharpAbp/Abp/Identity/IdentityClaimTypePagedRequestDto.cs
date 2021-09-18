using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.Identity
{
    public class IdentityClaimTypePagedRequestDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
