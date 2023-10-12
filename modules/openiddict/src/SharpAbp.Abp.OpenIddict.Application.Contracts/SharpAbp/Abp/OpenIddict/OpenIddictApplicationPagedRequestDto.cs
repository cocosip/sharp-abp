using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.OpenIddict
{
    public class OpenIddictApplicationPagedRequestDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
