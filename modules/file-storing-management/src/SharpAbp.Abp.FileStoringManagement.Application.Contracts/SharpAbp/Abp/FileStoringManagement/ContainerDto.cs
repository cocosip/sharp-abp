using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class ContainerDto : ExtensibleEntityDto<Guid>
    {
        public Guid? TenantId { get; set; }

        public bool IsMultiTenant { get; set; }

        public string Title { get; set; }

        public string Name { get; set; }

        public string Provider { get; set; }

        public bool HttpAccess { get; set; }

        public List<ContainerItemDto> Items { get; set; }

        public ContainerDto()
        {
            Items = new List<ContainerItemDto>();
        }
    }

    public class ContainerItemDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public Guid ContainerId { get; set; }
    }
}
