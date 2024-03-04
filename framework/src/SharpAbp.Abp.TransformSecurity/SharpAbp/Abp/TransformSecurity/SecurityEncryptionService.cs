using Microsoft.Extensions.Options;
using SharpAbp.Abp.Crypto.RSA;
using SharpAbp.Abp.Crypto.SM2;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.TransformSecurity
{
    public class SecurityEncryptionService : ISecurityEncryptionService, ITransientDependency
    {
        protected AbpTransformSecurityOptions Options { get; }
        protected AbpTransformSecurityRSAOptions RSAOptions { get; }
        protected AbpTransformSecuritySM2Options SM2Options { get; }
        protected IGuidGenerator GuidGenerator { get; }
        protected IClock Clock { get; }
        protected ISecurityCredentialStore  SecurityCredentialStore { get; }
        protected IRSAEncryptionService RSAEncryptionService { get; }
        protected ISm2EncryptionService Sm2EncryptionService { get; }
        public SecurityEncryptionService(
            IOptions<AbpTransformSecurityOptions> options,
            IOptions<AbpTransformSecurityRSAOptions> rsaOptions,
            IOptions<AbpTransformSecuritySM2Options> sm2Options,
            IGuidGenerator guidGenerator,
            IClock clock,
            ISecurityCredentialStore securityCredentialStore,
            IRSAEncryptionService rSAEncryptionService,
            ISm2EncryptionService sm2EncryptionService)
        {
            Options = options.Value;
            RSAOptions = rsaOptions.Value;
            SM2Options = sm2Options.Value;
            GuidGenerator = guidGenerator;
            Clock = clock;
            SecurityCredentialStore = securityCredentialStore;
            RSAEncryptionService = rSAEncryptionService;
            Sm2EncryptionService = sm2EncryptionService;
        }

        public virtual async Task<SecurityCredential> GenerateAsync(CancellationToken cancellationToken = default)
        {
            var credential = new SecurityCredential()
            {
                Identifier = GuidGenerator.Create().ToString("N"),
                Expires = Clock.Now.Add(Options.Expires),
                CreationTime = Clock.Now
            };

            if (Options.EncryptionAlgo == AbpTransformSecurityNames.RSA)
            {
                credential.KeyType = AbpTransformSecurityNames.RSA;
                var keyPair = RSAEncryptionService.GenerateRSAKeyPair(RSAOptions.KeySize);
                credential.PublicKey = RSAExtensions.ExportPublicKey(keyPair.Public);
                credential.PrivateKey = RSAExtensions.ExportPrivateKey(keyPair.Private);
                credential.SetRSAKeySize(RSAOptions.KeySize);
                credential.SetRSAPadding(RSAOptions.Padding);
            }
            else if (Options.EncryptionAlgo == AbpTransformSecurityNames.SM2)
            {
                credential.KeyType = AbpTransformSecurityNames.SM2;
                var keyPair = Sm2EncryptionService.GenerateSm2KeyPair(SM2Options.Curve);
                credential.PublicKey = Sm2Extensions.ExportPublicKey(keyPair.Public);
                credential.PrivateKey = Sm2Extensions.ExportPrivateKey(keyPair.Private);
                credential.SetSM2Curve(SM2Options.Curve);
                credential.SetSM2Mode(SM2Options.Mode);
            }

            await SecurityCredentialStore.SetAsync(credential);
            return credential;
        }

        public virtual async Task<string> DecryptAsync(string cipherText, string id, CancellationToken cancellationToken = default)
        {
            var credential = await SecurityCredentialStore.GetAsync(id, cancellationToken);
            if (credential == null)
            {
                throw new AbpException($"Could not find security key by id: {id}");
            }
            if (credential.IsRSA())
            {
                var rsaParam = RSAEncryptionService.ImportPrivateKey(credential.PrivateKey);
                return RSAEncryptionService.Decrypt(rsaParam, cipherText, Encoding.UTF8, credential.GetRSAPadding());

            }
            else if (credential.IsSM2())
            {
                return Sm2EncryptionService.Decrypt(credential.PrivateKey, cipherText, "utf-8", credential.GetSM2Curve(), credential.GetSM2Mode());
            }
            else
            {
                throw new AbpException("Invalid credential key type");
            }

        }

    }
}
