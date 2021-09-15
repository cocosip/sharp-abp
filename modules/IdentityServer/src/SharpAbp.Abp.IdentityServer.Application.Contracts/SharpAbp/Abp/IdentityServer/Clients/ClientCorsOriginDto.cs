using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.IdentityServer.Clients
{
    public class ClientCorsOriginDto : EntityDto
    {
        public Guid ClientId { get; set; }
        public string Origin { get; set; }
    }
}
