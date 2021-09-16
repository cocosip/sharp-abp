using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.IdentityServer.ApiResources
{
    public class ApiResourcePropertyDto : EntityDto
    {
        public Guid ApiResourceId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
