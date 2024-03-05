using Microsoft.Extensions.Options;
using SharpAbp.Abp.Crypto.RSA;
using SharpAbp.Abp.Crypto.SM2;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.TransformSecurity
{
    public class SecurityCredentialManager : ISecurityCredentialManager, ITransientDependency
    {
        protected AbpTransformSecurityOptions Options { get; }
        protected AbpTransformSecurityRSAOptions RSAOptions { get; }
        protected AbpTransformSecuritySM2Options SM2Options { get; }
        protected IGuidGenerator GuidGenerator { get; }
        protected IClock Clock { get; }
        protected ISecurityCredentialStore SecurityCredentialStore { get; }
        protected IRSAEncryptionService RSAEncryptionService { get; }
        protected ISm2EncryptionService Sm2EncryptionService { get; }
        public SecurityCredentialManager(
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
                Expires = Clock.Now.Add(Options.Expires),
                CreationTime = Clock.Now
            };

            if (Options.EncryptionAlgo.Equals("RSA", StringComparison.OrdinalIgnoreCase))
            {
                credential.KeyType = AbpTransformSecurityNames.RSA;
                var keyPair = RSAEncryptionService.GenerateRSAKeyPair(RSAOptions.KeySize);
                credential.PublicKey = RSAExtensions.ExportPublicKey(keyPair.Public);
                credential.PrivateKey = RSAExtensions.ExportPrivateKey(keyPair.Private);
                credential.SetRSAKeySize(RSAOptions.KeySize);
                credential.SetRSAPadding(RSAOptions.Padding);
            }
            else if (Options.EncryptionAlgo.Equals("SM2", StringComparison.OrdinalIgnoreCase))
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
