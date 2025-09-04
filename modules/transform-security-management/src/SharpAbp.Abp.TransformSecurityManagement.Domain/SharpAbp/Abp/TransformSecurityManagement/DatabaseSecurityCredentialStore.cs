using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.CryptoVault;
using SharpAbp.Abp.TransformSecurity;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    [Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
    [ExposeServices(typeof(ISecurityCredentialStore))]
    public class DatabaseSecurityCredentialStore : ISecurityCredentialStore, ITransientDependency
    {
        protected IGuidGenerator GuidGenerator { get; }
        protected IKeyService KeyService { get; }
        protected ISecurityCredentialInfoRepository SecurityCredentialInfoRepository { get; }
        protected IRSACredsRepository RSACredsRepository { get; }
        protected ISM2CredsRepository SM2CredsRepository { get; }
        public DatabaseSecurityCredentialStore(
            IGuidGenerator guidGenerator,
            IKeyService keyService,
            ISecurityCredentialInfoRepository
            securityCredentialInfoRepository,
            IRSACredsRepository rsaCredsRepository,
            ISM2CredsRepository sm2CredsRepository)
        {
            GuidGenerator = guidGenerator;
            KeyService = keyService;
            SecurityCredentialInfoRepository = securityCredentialInfoRepository;
            RSACredsRepository = rsaCredsRepository;
            SM2CredsRepository = sm2CredsRepository;
        }

        public virtual async Task<SecurityCredential> GetAsync(string identifier, CancellationToken cancellationToken = default)
        {
            var securityCredentialInfo = await SecurityCredentialInfoRepository.FindByIdentifierAsync(identifier, cancellationToken: cancellationToken);
            if (securityCredentialInfo == null)
            {
                throw new AbpException($"Security credential info with identifier '{identifier}' could not be found.");
            }

            var securityCredential = new SecurityCredential()
            {
                Identifier = securityCredentialInfo.Identifier,
                KeyType = securityCredentialInfo.KeyType,
                BizType = securityCredentialInfo.BizType,
                Expires = securityCredentialInfo.Expires,
                CreationTime = securityCredentialInfo.CreationTime,
            };

            securityCredential.SetReferenceId(securityCredentialInfo.CredsId.ToString("N"));

            if (securityCredential.KeyType.Equals("RSA", StringComparison.OrdinalIgnoreCase))
            {
                var rsaCreds = await RSACredsRepository.GetAsync(securityCredentialInfo.CredsId, cancellationToken: cancellationToken);
                securityCredential.PublicKey = KeyService.DecryptKey(rsaCreds.PublicKey, rsaCreds.PassPhrase, rsaCreds.Salt);
                securityCredential.PrivateKey = KeyService.DecryptKey(rsaCreds.PrivateKey, rsaCreds.PassPhrase, rsaCreds.Salt);
            }
            else if (securityCredentialInfo.KeyType.Equals("SM2", StringComparison.OrdinalIgnoreCase))
            {
                var sm2Creds = await SM2CredsRepository.GetAsync(securityCredentialInfo.CredsId, cancellationToken: cancellationToken);
                securityCredential.PublicKey = KeyService.DecryptKey(sm2Creds.PublicKey, sm2Creds.PassPhrase, sm2Creds.Salt);
                securityCredential.PrivateKey = KeyService.DecryptKey(sm2Creds.PrivateKey, sm2Creds.PassPhrase, sm2Creds.Salt);
            }
            else
            {
                throw new AbpException($"Unsupported key type '{securityCredentialInfo.KeyType}'. Only 'RSA' and 'SM2' key types are supported.");
            }
            return securityCredential;
        }

        public virtual async Task SetAsync(SecurityCredential credential, CancellationToken cancellationToken = default)
        {
            var referenceId = credential.GetReferenceId();
            if (referenceId.IsNullOrWhiteSpace())
            {
                throw new AbpException("The reference identifier of the security credential is null or empty. Please ensure the credential has a valid reference identifier before setting it.");
            }

            var securityCredentialInfo = new SecurityCredentialInfo(
                GuidGenerator.Create(),
                credential.Identifier,
                Guid.Parse(referenceId),
                credential.KeyType,
                credential.BizType,
                credential.Expires,
                "");

            await SecurityCredentialInfoRepository.InsertAsync(securityCredentialInfo, cancellationToken: cancellationToken);
        }
    }
}