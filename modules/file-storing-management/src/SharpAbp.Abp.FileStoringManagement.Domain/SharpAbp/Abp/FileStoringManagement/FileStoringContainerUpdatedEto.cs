using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace SharpAbp.Abp.FileStoringManagement
{
    [Serializable]
    public class FileStoringContainerUpdatedEto : EtoBase
    {
        public Guid Id { get; set; }
        public Guid? TenantId { get; set; }
        public string OldName { get; set; }
        public string Name { get; set; }
    }

}
