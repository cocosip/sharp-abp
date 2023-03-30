using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupPagedRequestDto : PagedAndSortedResultRequestDto
    {
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}
