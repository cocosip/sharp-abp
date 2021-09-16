using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.IdentityServer.Clients
{
    public class ClientPropertyDto : EntityDto
    {
        public Guid ClientId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
