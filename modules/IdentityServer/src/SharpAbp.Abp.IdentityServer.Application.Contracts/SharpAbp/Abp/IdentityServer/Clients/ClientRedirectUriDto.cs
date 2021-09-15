using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.IdentityServer.Clients
{
    public class ClientRedirectUriDto : EntityDto
    {
        public Guid ClientId { get; set; }

        public string RedirectUri { get; set; }
    }
}
