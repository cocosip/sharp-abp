using Volo.Abp.Application.Dtos;

namespace SharpAbp.MinId
{
    public class MinIdTokenPagedRequestDto : PagedAndSortedResultRequestDto
    {
        public string BizType { get; set; }

        public string Token { get; set; }
    }
}
