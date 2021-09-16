using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.IdentityServer.Clients
{
    public class ClientClaimDto : EntityDto
    {
        public Guid ClientId { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }
    }
}
