using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    /// <summary>
    /// Data transfer object for requesting paged and filtered database connection information.
    /// Extends PagedAndSortedResultRequestDto to support pagination, sorting, and filtering capabilities.
    /// Used in queries to retrieve database connections with specific criteria.
    /// </summary>
    public class DatabaseConnectionInfoPagedRequestDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Gets or sets the name filter for database connections.
        /// When specified, only connections with names containing this value will be returned.
        /// </summary>
        /// <value>A string used to filter connections by name, can be null for no name filtering.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the database provider filter for database connections.
        /// When specified, only connections using this provider type will be returned.
        /// </summary>
        /// <value>A string representing the database provider to filter by, can be null for no provider filtering.</value>
        public string DatabaseProvider { get; set; }
    }
}
