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
using static SharpAbp.Abp.Crypto.RSA.RSAPaddingNames;

namespace SharpAbp.Abp.TransformSecurity
{
    public class SecurityEncryptionService : ISecurityEncryptionService, ITransientDependency
    {
        protected AbpTransformSecurityOptions Options { get; }
        protected AbpTransformSecurityRSAOptions RSAOptions { get; }
        protected AbpTransformSecuritySM2Options SM2Options { get; }
        protected IGuidGenerator GuidGenerator { get; }
        protected IClock Clock { get; }
        protected ISecurityCredentialStore SecurityCredentialStore { get; }
        protected IRSAEncryptionService RSAEncryptionService { get; }
        protected ISm2EncryptionService Sm2EncryptionService { get; }
        public SecurityEncryptionService(
            IOptions<AbpTransformSecurityOptions> options,
            IOptions<AbpTransformSecurityRSAOptions> rsaOptions,
            IOptions<AbpTransformSecuritySM2Options> sm2Options,
            IGuidGenerator guidGenerator,
            IClock clock,
            ISecurityCredentialStore securityCredentialStore,
            IRSAEncryptionService rsaEncryptionService,
            ISm2EncryptionService sm2EncryptionService)
        {
            Options = options.Value;
            RSAOptions = rsaOptions.Value;
            SM2Options = sm2Options.Value;
            GuidGenerator = guidGenerator;
            Clock = clock;
            SecurityCredentialStore = securityCredentialStore;
            RSAEncryptionService = rsaEncryptionService;
            Sm2EncryptionService = sm2EncryptionService;
        }

        public virtual async Task<SecurityCredentialValidateResult> ValidateAsync(string identifier, CancellationToken cancellationToken = default)
        {
            var result = new SecurityCredentialValidateResult();
            var credential = await SecurityCredentialStore.GetAsync(identifier, cancellationToken);
            if (credential == null)
            {
                result.Result = SecurityCredentialResultType.NotFound;
                return result;
            }
            if (credential.IsExpires(Clock.Now))
            {
                result.Result = SecurityCredentialResultType.Expired;
                return result;
            }
            result.Result = SecurityCredentialResultType.Success;
            return result;
        }

        public virtual async Task<string> EncryptAsync(string plainText, string identifier, CancellationToken cancellationToken = default)
        {
            var credential = await SecurityCredentialStore.GetAsync(identifier, cancellationToken);
            if (credential == null)
            {
                throw new AbpException($"Could not find security key by id: {identifier}");
            }
            if (credential.IsRSA())
            {
                return RSAEncryptionService.EncryptFromBase64(credential.PublicKey!, plainText, Encoding.UTF8, credential.GetRSAPadding() ?? None);
            }
            else if (credential.IsSM2())
            {
                return Sm2EncryptionService.Encrypt(credential.PublicKey!, plainText, Encoding.UTF8, SM2Options.Curve, SM2Options.Mode);
            }
            else
            {
                throw new AbpException("Invalid credential key type");
            }
        }

        public virtual async Task<string> DecryptAsync(string cipherText, string identifier, CancellationToken cancellationToken = default)
        {
            var credential = await SecurityCredentialStore.GetAsync(identifier, cancellationToken);
            if (credential == null)
            {
                throw new AbpException($"Could not find security key by id: {identifier}");
            }
            if (credential.IsRSA())
            {
                return RSAEncryptionService.DecryptFromBase64(credential.PrivateKey!, cipherText, Encoding.UTF8, credential.GetRSAPadding() ?? None);
            }
            else if (credential.IsSM2())
            {
                return Sm2EncryptionService.Decrypt(credential.PrivateKey!, cipherText, Encoding.UTF8, SM2Options.Curve, SM2Options.Mode);
            }
            else
            {
                throw new AbpException("Invalid credential key type");
            }

        }

    }
}
