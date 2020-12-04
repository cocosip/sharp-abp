using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringContainerPagedRequestDto : PagedAndSortedResultRequestDto
    {
        public string Name { get; set; }

        public string Provider { get; set; }

    }
}
