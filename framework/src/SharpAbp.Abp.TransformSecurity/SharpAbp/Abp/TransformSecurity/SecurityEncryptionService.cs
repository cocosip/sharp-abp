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
    /// <summary>
    /// Default implementation of <see cref="ISecurityEncryptionService"/> for handling encryption and decryption operations
    /// </summary>
    public class SecurityEncryptionService : ISecurityEncryptionService, ITransientDependency
    {
        /// <summary>
        /// Gets the transform security options
        /// </summary>
        protected AbpTransformSecurityOptions Options { get; }
        
        /// <summary>
        /// Gets the RSA encryption options
        /// </summary>
        protected AbpTransformSecurityRSAOptions RSAOptions { get; }
        
        /// <summary>
        /// Gets the SM2 encryption options
        /// </summary>
        protected AbpTransformSecuritySM2Options SM2Options { get; }
        
        /// <summary>
        /// Gets the GUID generator service
        /// </summary>
        protected IGuidGenerator GuidGenerator { get; }
        
        /// <summary>
        /// Gets the clock service for time operations
        /// </summary>
        protected IClock Clock { get; }
        
        /// <summary>
        /// Gets the security credential store service
        /// </summary>
        protected ISecurityCredentialStore SecurityCredentialStore { get; }
        
        /// <summary>
        /// Gets the RSA encryption service
        /// </summary>
        protected IRSAEncryptionService RSAEncryptionService { get; }
        
        /// <summary>
        /// Gets the SM2 encryption service
        /// </summary>
        protected ISm2EncryptionService Sm2EncryptionService { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityEncryptionService"/> class
        /// </summary>
        /// <param name="options">The transform security options</param>
        /// <param name="rsaOptions">The RSA encryption options</param>
        /// <param name="sm2Options">The SM2 encryption options</param>
        /// <param name="guidGenerator">The GUID generator service</param>
        /// <param name="clock">The clock service</param>
        /// <param name="securityCredentialStore">The security credential store service</param>
        /// <param name="rsaEncryptionService">The RSA encryption service</param>
        /// <param name="sm2EncryptionService">The SM2 encryption service</param>
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

        /// <summary>
        /// Validates a security credential by its identifier
        /// </summary>
        /// <param name="identifier">The unique identifier of the security credential</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation and contains the validation result</returns>
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

        /// <summary>
        /// Encrypts the specified plain text using the security credential identified by the given identifier
        /// </summary>
        /// <param name="plainText">The plain text to encrypt</param>
        /// <param name="identifier">The unique identifier of the security credential</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation and contains the encrypted text</returns>
        /// <exception cref="AbpException">Thrown when the security credential is not found or has an invalid key type</exception>
        public virtual async Task<string> EncryptAsync(string plainText, string identifier, CancellationToken cancellationToken = default)
        {
            var credential = await SecurityCredentialStore.GetAsync(identifier, cancellationToken);
            if (credential == null)
            {
                throw new AbpException($"Security credential with identifier '{identifier}' was not found. Please ensure the credential exists and is valid.");
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
                throw new AbpException($"The security credential with identifier '{identifier}' has an unsupported key type '{credential.KeyType}'. Supported key types are RSA and SM2.");
            }
        }

        /// <summary>
        /// Decrypts the specified cipher text using the security credential identified by the given identifier
        /// </summary>
        /// <param name="cipherText">The cipher text to decrypt</param>
        /// <param name="identifier">The unique identifier of the security credential</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation and contains the decrypted plain text</returns>
        /// <exception cref="AbpException">Thrown when the security credential is not found or has an invalid key type</exception>
        public virtual async Task<string> DecryptAsync(string cipherText, string identifier, CancellationToken cancellationToken = default)
        {
            var credential = await SecurityCredentialStore.GetAsync(identifier, cancellationToken);
            if (credential == null)
            {
                throw new AbpException($"Security credential with identifier '{identifier}' was not found. Please ensure the credential exists and is valid.");
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
                throw new AbpException($"The security credential with identifier '{identifier}' has an unsupported key type '{credential.KeyType}'. Supported key types are RSA and SM2.");
            }
        }

    }
}
