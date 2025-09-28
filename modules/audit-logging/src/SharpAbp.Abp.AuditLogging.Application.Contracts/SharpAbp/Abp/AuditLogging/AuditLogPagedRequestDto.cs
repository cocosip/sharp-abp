using System;
using System.Net;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.AuditLogging
{
    /// <summary>
    /// Data transfer object for paged audit log requests with filtering options
    /// </summary>
    public class AuditLogPagedRequestDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Gets or sets the start time for filtering audit logs
        /// </summary>
        public DateTime? StartTime { get; set; }
        
        /// <summary>
        /// Gets or sets the end time for filtering audit logs
        /// </summary>
        public DateTime? EndTime { get; set; }
        
        /// <summary>
        /// Gets or sets the HTTP method filter for audit logs
        /// </summary>
        public string HttpMethod { get; set; }
        
        /// <summary>
        /// Gets or sets the URL filter for audit logs
        /// </summary>
        public string Url { get; set; }
        
        /// <summary>
        /// Gets or sets the client identifier filter for audit logs
        /// </summary>
        public string ClientId { get; set; }
        
        /// <summary>
        /// Gets or sets the user identifier filter for audit logs
        /// </summary>
        public Guid? UserId { get; set; }
        
        /// <summary>
        /// Gets or sets the user name filter for audit logs
        /// </summary>
        public string UserName { get; set; }
        
        /// <summary>
        /// Gets or sets the application name filter for audit logs
        /// </summary>
        public string ApplicationName { get; set; }
        
        /// <summary>
        /// Gets or sets the client IP address filter for audit logs
        /// </summary>
        public string ClientIpAddress { get; set; }
        
        /// <summary>
        /// Gets or sets the correlation identifier filter for audit logs
        /// </summary>
        public string CorrelationId { get; set; }
        
        /// <summary>
        /// Gets or sets the maximum execution duration filter in milliseconds
        /// </summary>
        public int? MaxExecutionDuration { get; set; }
        
        /// <summary>
        /// Gets or sets the minimum execution duration filter in milliseconds
        /// </summary>
        public int? MinExecutionDuration { get; set; }
        
        /// <summary>
        /// Gets or sets the filter for audit logs that have exceptions
        /// </summary>
        public bool? HasException { get; set; }
        
        /// <summary>
        /// Gets or sets the HTTP status code filter for audit logs
        /// </summary>
        public HttpStatusCode? HttpStatusCode { get; set; }
    }
}
