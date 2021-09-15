using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.IdentityServer.Clients
{
    public class ClientGrantTypeDto : EntityDto
    {
        public Guid ClientId { get; set; }
        public string GrantType { get; set; }
    }
}
