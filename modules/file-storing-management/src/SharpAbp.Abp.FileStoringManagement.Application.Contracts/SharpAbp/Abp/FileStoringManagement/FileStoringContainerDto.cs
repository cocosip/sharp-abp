using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringContainerDto : EntityDto<Guid>
    {
        public Guid? TenantId { get; set; }

        public bool IsMultiTenant { get; set; }

        public string Title { get; set; }

        public string Name { get; set; }

        public string Provider { get; set; }

        public bool HttpSupport { get; set; }

        public List<FileStoringContainerItemDto> Items { get; set; }

        public FileStoringContainerDto()
        {
            Items = new List<FileStoringContainerItemDto>();
        }
    }

    public class FileStoringContainerItemDto : EntityDto<Guid>
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public string TypeName { get; set; }

        public Guid ContainerId { get; set; }
    }
}
