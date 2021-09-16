using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.IdentityServer.ApiResources
{
    public class ApiResourceScopeDto : EntityDto
    {
        public Guid ApiResourceId { get; set; }

        public string Scope { get; set; }
    }
}
