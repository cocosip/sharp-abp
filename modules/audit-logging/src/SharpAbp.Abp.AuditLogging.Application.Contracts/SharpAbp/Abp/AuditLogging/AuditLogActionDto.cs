using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;
using Volo.Abp.Data;

namespace SharpAbp.Abp.AuditLogging
{
    /// <summary>
    /// Data transfer object for audit log action information
    /// </summary>
    [DisableAuditing]
    public class AuditLogActionDto : EntityDto<Guid>
    {
        /// <summary>
        /// Gets or sets the tenant identifier for multi-tenant applications
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Gets or sets the audit log identifier that this action belongs to
        /// </summary>
        public Guid AuditLogId { get; set; }

        /// <summary>
        /// Gets or sets the service name where the action was performed
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the method name that was executed
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets the parameters passed to the method in JSON format
        /// </summary>
        public string Parameters { get; set; }

        /// <summary>
        /// Gets or sets the execution time when the action was performed
        /// </summary>
        public DateTime ExecutionTime { get; set; }

        /// <summary>
        /// Gets or sets the execution duration in milliseconds
        /// </summary>
        public int ExecutionDuration { get; set; }

        /// <summary>
        /// Gets or sets the extra properties for extending the audit log action
        /// </summary>
        public ExtraPropertyDictionary ExtraProperties { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogActionDto"/> class
        /// </summary>
        public AuditLogActionDto()
        {
            ExtraProperties = new ExtraPropertyDictionary();
        }
    }
}
