using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.OpenIddict
{
    /// <summary>
    /// Data transfer object for paged and sorted OpenIddict scope requests
    /// </summary>
    public class OpenIddictScopePagedRequestDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Filter string for searching scopes by name or other criteria
        /// </summary>
        public string Filter { get; set; }
    }
}
