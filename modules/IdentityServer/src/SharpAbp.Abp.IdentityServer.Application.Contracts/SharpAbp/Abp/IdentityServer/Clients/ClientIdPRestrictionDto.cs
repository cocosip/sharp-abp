using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.IdentityServer.Clients
{
    public class ClientIdPRestrictionDto : EntityDto
    {
        public Guid ClientId { get; set; }

        public string Provider { get; set; }

    }
}
