using Microsoft.Extensions.Options;
using SharpAbp.Abp.Crypto.RSA;
using SharpAbp.Abp.Crypto.SM2;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.TransformSecurity
{
    public class SecurityKeyManager : ISecurityKeyManager, ITransientDependency
    {
        protected AbpTransformSecurityOptions Options { get; }
        protected AbpTransformSecurityRSAOptions RSAOptions { get; }
        protected AbpTransformSecuritySM2Options SM2Options { get; }
        protected IGuidGenerator GuidGenerator { get; }
        protected IClock Clock { get; }
        protected ISecurityKeyStore SecurityKeyStore { get; }
        protected IRSAEncryptionService RSAEncryptionService { get; }
        protected ISm2EncryptionService Sm2EncryptionService { get; }
        public SecurityKeyManager(
            IOptions<AbpTransformSecurityOptions> options,
            IOptions<AbpTransformSecurityRSAOptions> rsaOptions,
            IOptions<AbpTransformSecuritySM2Options> sm2Options,
            IGuidGenerator guidGenerator,
            IClock clock,
            ISecurityKeyStore securityKeyStore,
            IRSAEncryptionService rSAEncryptionService,
            ISm2EncryptionService sm2EncryptionService)
        {
            Options = options.Value;
            RSAOptions = rsaOptions.Value;
            SM2Options = sm2Options.Value;
            GuidGenerator = guidGenerator;
            Clock = clock;
            SecurityKeyStore = securityKeyStore;
            RSAEncryptionService = rSAEncryptionService;
            Sm2EncryptionService = sm2EncryptionService;
        }

        public virtual async Task<SecurityKey> GenerateAsync(CancellationToken cancellationToken = default)
        {
            var securityKey = new SecurityKey()
            {
                UniqueId = GuidGenerator.Create().ToString("N"),
                Expires = Clock.Now.Add(Options.Expires),
                CreationTime = Clock.Now
            };

            if (Options.EncryptionAlgo == "RSA")
            {
                securityKey.KeyType = AbpTransformSecurityNames.RSA;
                var keyPair = RSAEncryptionService.GenerateRSAKeyPair(RSAOptions.KeySize);
                securityKey.PublicKey = RSAExtensions.ExportPublicKey(keyPair.Public);
                securityKey.PrivateKey = RSAExtensions.ExportPrivateKey(keyPair.Private);
                securityKey.SetRSAKeySize(RSAOptions.KeySize);
                securityKey.SetRSAPadding(RSAOptions.Padding);
            }
            else if (Options.EncryptionAlgo == "SM2")
            {
                securityKey.KeyType = AbpTransformSecurityNames.SM2;
                var keyPair = Sm2EncryptionService.GenerateSm2KeyPair(SM2Options.Curve);
                securityKey.PublicKey = Sm2Extensions.ExportPublicKey(keyPair.Public);
                securityKey.PrivateKey = Sm2Extensions.ExportPrivateKey(keyPair.Private);
                securityKey.SetSM2Curve(SM2Options.Curve);
                securityKey.SetSM2Mode(SM2Options.Mode);
            }

            await SecurityKeyStore.SetAsync(securityKey);
            return securityKey;
        }

    }
}
