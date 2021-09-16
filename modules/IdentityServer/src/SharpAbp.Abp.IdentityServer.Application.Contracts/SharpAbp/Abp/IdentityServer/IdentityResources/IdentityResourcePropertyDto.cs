using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.IdentityServer.IdentityResources
{
    public class IdentityResourcePropertyDto : EntityDto
    {
        public Guid IdentityResourceId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
