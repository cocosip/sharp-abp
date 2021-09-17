using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer.Clients
{
    public class CreateOrUpdateClientScopeDto
    {
        public Guid? ClientId { get; set; }

        [Required]
        [DynamicStringLength(typeof(ClientScopeConsts), nameof(ClientScopeConsts.ScopeMaxLength))]
        public string Scope { get; set; }
    }
}
