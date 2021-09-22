using JetBrains.Annotations;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.IdentityServer.Clients;

namespace SharpAbp.Abp.IdentityServer
{
    public class IdentityServerClientManager : DomainService, IIdentityServerClientManager
    {
        protected IClientRepository ClientRepository { get; }
        public IdentityServerClientManager(IClientRepository clientRepository)
        {
            ClientRepository = clientRepository;
        }


        /// <summary>
        /// Validate clientId
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public virtual async Task ValidateClientIdAsync([NotNull] string clientId)
        {
            Check.NotNullOrWhiteSpace(clientId, nameof(clientId));
            var client = await ClientRepository.FindByClientIdAsync(clientId, false, default);
            if (client == null)
            {
                throw new AbpException($"Dumplicate clientId {clientId}.");
            }
        }
    }
}
