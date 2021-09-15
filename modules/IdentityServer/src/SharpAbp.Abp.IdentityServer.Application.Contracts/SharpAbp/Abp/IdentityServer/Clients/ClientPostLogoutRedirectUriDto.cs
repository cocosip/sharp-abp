using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.IdentityServer.Clients
{
    public class ClientPostLogoutRedirectUriDto : EntityDto
    {
        public Guid ClientId { get; set; }

        public string PostLogoutRedirectUri { get; set; }
    }
}
