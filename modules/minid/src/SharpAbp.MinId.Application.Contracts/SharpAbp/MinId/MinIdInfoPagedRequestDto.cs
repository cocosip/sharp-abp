using Volo.Abp.Application.Dtos;

namespace SharpAbp.MinId
{
    public class MinIdInfoPagedRequestDto : PagedAndSortedResultRequestDto
    {
        public string BizType { get; set; }
    }
}
