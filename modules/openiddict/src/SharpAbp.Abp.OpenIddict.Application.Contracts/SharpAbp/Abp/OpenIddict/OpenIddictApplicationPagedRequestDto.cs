using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.OpenIddict
{
    /// <summary>
    /// Data transfer object for paged and sorted OpenIddict application requests
    /// </summary>
    public class OpenIddictApplicationPagedRequestDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Filter string for searching applications by name or other criteria
        /// </summary>
        public string Filter { get; set; }
    }
}
