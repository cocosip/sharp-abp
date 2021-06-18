using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class DatabaseConnectionInfoPagedRequestDto : PagedAndSortedResultRequestDto
    {
        public string Name { get; set; }

        public string DatabaseProvider { get; set; }
    }
}
