using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.IdentityServer.ApiScopes
{
    public class ApiScopePropertyDto : EntityDto
    {
        public Guid ApiScopeId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
