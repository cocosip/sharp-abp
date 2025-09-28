using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;

namespace SharpAbp.Abp.AuditLogging
{
    /// <summary>
    /// Data transfer object for audit log information
    /// </summary>
    [DisableAuditing]
    public class AuditLogDto : ExtensibleEntityDto<Guid>
    {
        /// <summary>
        /// Gets or sets the application name that generated this audit log
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the user identifier who performed the action
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Gets or sets the user name who performed the action
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier for multi-tenant applications
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Gets or sets the tenant name for multi-tenant applications
        /// </summary>
        public string TenantName { get; set; }

        /// <summary>
        /// Gets or sets the impersonator user identifier if the action was performed on behalf of another user
        /// </summary>
        public Guid? ImpersonatorUserId { get; set; }

        /// <summary>
        /// Gets or sets the impersonator tenant identifier if the action was performed on behalf of another tenant
        /// </summary>
        public Guid? ImpersonatorTenantId { get; set; }

        /// <summary>
        /// Gets or sets the impersonator tenant name if the action was performed on behalf of another tenant
        /// </summary>
        public string ImpersonatorTenantName { get; set; }

        /// <summary>
        /// Gets or sets the execution time when the action was performed
        /// </summary>
        public DateTime ExecutionTime { get; set; }

        /// <summary>
        /// Gets or sets the execution duration in milliseconds
        /// </summary>
        public int ExecutionDuration { get; set; }

        /// <summary>
        /// Gets or sets the client IP address from which the action was performed
        /// </summary>
        public string ClientIpAddress { get; set; }

        /// <summary>
        /// Gets or sets the client name or application that performed the action
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Gets or sets the client identifier
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the correlation identifier for tracking related operations
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the browser information from which the action was performed
        /// </summary>
        public string BrowserInfo { get; set; }

        /// <summary>
        /// Gets or sets the HTTP method used for the request
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// Gets or sets the URL that was accessed
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the exception information if any error occurred during execution
        /// </summary>
        public string Exceptions { get; set; }

        /// <summary>
        /// Gets or sets additional comments for the audit log
        /// </summary>
        public string Comments { get; protected set; }

        /// <summary>
        /// Gets or sets the HTTP status code returned by the request
        /// </summary>
        public int? HttpStatusCode { get; set; }

        /// <summary>
        /// Gets or sets the list of entity changes that occurred during the action
        /// </summary>
        public List<EntityChangeDto> EntityChanges { get; set; }

        /// <summary>
        /// Gets or sets the list of actions performed during the audit log
        /// </summary>
        public List<AuditLogActionDto> Actions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogDto"/> class
        /// </summary>
        public AuditLogDto()
        {
            EntityChanges = new List<EntityChangeDto>();
            Actions = new List<AuditLogActionDto>();
        }
    }
}
