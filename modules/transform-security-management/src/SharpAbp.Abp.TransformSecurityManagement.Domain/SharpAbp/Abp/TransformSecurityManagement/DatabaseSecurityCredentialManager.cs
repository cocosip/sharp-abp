using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.CryptoVault;
using SharpAbp.Abp.TransformSecurity;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    [Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
    [ExposeServices(typeof(ISecurityCredentialManager))]
    public class DatabaseSecurityCredentialManager : ISecurityCredentialManager, ITransientDependency
    {
        protected AbpTransformSecurityOptions Options { get; }
        protected AbpTransformSecurityRSAOptions RSAOptions { get; }
        protected AbpTransformSecuritySM2Options SM2Options { get; }
        protected IGuidGenerator GuidGenerator { get; }
        protected IClock Clock { get; }
        protected IKeyService KeyService { get; }
        protected ISecurityCredentialStore SecurityCredentialStore { get; }
        protected IRSACredsRepository RSACredsRepository { get; }
        protected ISM2CredsRepository SM2CredsRepository { get; }
        public DatabaseSecurityCredentialManager(
            IOptions<AbpTransformSecurityOptions> options,
            IOptions<AbpTransformSecurityRSAOptions> rsaOptions,
            IOptions<AbpTransformSecuritySM2Options> sm2Options,
            IGuidGenerator guidGenerator,
            IClock clock,
            IKeyService keyService,
            ISecurityCredentialStore securityCredentialStore,
            IRSACredsRepository rsaCredsRepository,
            ISM2CredsRepository sm2CredsRepository)
        {
            Options = options.Value;
            RSAOptions = rsaOptions.Value;
            SM2Options = sm2Options.Value;
            GuidGenerator = guidGenerator;
            Clock = clock;
            KeyService = keyService;
            SecurityCredentialStore = securityCredentialStore;
            RSACredsRepository = rsaCredsRepository;
            SM2CredsRepository = sm2CredsRepository;
        }

        public virtual async Task<SecurityCredential> GenerateAsync(string bizType, CancellationToken cancellationToken = default)
        {
            if (!ValidateBizType(bizType))
            {
                throw new AbpException($"Unsupported bizType {bizType}");
            }

            var credential = new SecurityCredential()
            {
                Identifier = GuidGenerator.Create().ToString("N"),
                BizType = bizType,
                KeyType = Options.EncryptionAlgo,
                Expires = Clock.Now.Add(Options.Expires),
                CreationTime = Clock.Now
            };
            if (Options.EncryptionAlgo.Equals("RSA", StringComparison.OrdinalIgnoreCase))
            {
                credential.KeyType = "RSA";
                var rsaCreds = await RSACredsRepository.GetRandomAsync(size: credential.GetRSAKeySize(), cancellationToken: cancellationToken) ?? throw new AbpException("Get RSACreds failed");

                credential.SetReferenceId(rsaCreds.Id.ToString("N"));
                credential.SetSM2Curve(SM2Options.Curve);
                credential.SetSM2Mode(SM2Options.Mode);
                credential.PublicKey = KeyService.DecryptKey(rsaCreds.PublicKey, rsaCreds.PassPhrase, rsaCreds.Salt);
                credential.PrivateKey = KeyService.DecryptKey(rsaCreds.PrivateKey, rsaCreds.PassPhrase, rsaCreds.Salt);

            }
            else if (Options.EncryptionAlgo.Equals("SM2", StringComparison.OrdinalIgnoreCase))
            {
                credential.KeyType = "SM2";
                var sm2Creds = await SM2CredsRepository.GetRandomAsync(curve: credential.GetSM2Curve(), cancellationToken: cancellationToken) ?? throw new AbpException("Get SM2Creds failed");

                credential.SetReferenceId(sm2Creds.Id.ToString("N"));
                credential.SetRSAKeySize(RSAOptions.KeySize);
                credential.SetRSAPadding(RSAOptions.Padding);
                credential.PublicKey = KeyService.DecryptKey(sm2Creds.PublicKey, sm2Creds.PassPhrase, sm2Creds.Salt);
                credential.PrivateKey = KeyService.DecryptKey(sm2Creds.PrivateKey, sm2Creds.PassPhrase, sm2Creds.Salt);
            }
            else
            {
                throw new AbpException($"Unsupport EncryptionAlgo {Options.EncryptionAlgo}");
            }
            await SecurityCredentialStore.SetAsync(credential);
            return credential;
        }

        protected virtual bool ValidateBizType(string bizType)
        {
            foreach (var item in Options.BizTypes)
            {
                if (item.Equals(bizType, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
