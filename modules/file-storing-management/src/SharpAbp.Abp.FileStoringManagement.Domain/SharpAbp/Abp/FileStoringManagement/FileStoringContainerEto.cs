using System;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringContainerEto : EntityEto, IMultiTenant
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// TenantId
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Provider
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// Container name
        /// </summary>
        public string Name { get; set; }
    }
}
