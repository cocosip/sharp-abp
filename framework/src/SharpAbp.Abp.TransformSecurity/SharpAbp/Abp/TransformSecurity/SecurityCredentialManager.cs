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
    /// <summary>
    /// Default implementation of <see cref="ISecurityCredentialManager"/> for managing security credentials
    /// </summary>
    public class SecurityCredentialManager : ISecurityCredentialManager, ITransientDependency
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
        /// Initializes a new instance of the <see cref="SecurityCredentialManager"/> class
        /// </summary>
        /// <param name="options">The transform security options</param>
        /// <param name="rsaOptions">The RSA encryption options</param>
        /// <param name="sm2Options">The SM2 encryption options</param>
        /// <param name="guidGenerator">The GUID generator service</param>
        /// <param name="clock">The clock service</param>
        /// <param name="securityCredentialStore">The security credential store service</param>
        /// <param name="rsaEncryptionService">The RSA encryption service</param>
        /// <param name="sm2EncryptionService">The SM2 encryption service</param>
        public SecurityCredentialManager(
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
        /// Generates a new security credential for the specified business type
        /// </summary>
        /// <param name="bizType">The business type for which to generate the credential</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation and contains the generated security credential</returns>
        /// <exception cref="AbpException">Thrown when the business type is not supported</exception>
        public virtual async Task<SecurityCredential> GenerateAsync(string bizType, CancellationToken cancellationToken = default)
        {
            if (!ValidateBizType(bizType))
            {
                throw new AbpException($"The business type '{bizType}' is not supported. Please ensure the business type is configured in the supported business types list.");
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

        /// <summary>
        /// Validates whether the specified business type is supported
        /// </summary>
        /// <param name="bizType">The business type to validate</param>
        /// <returns>True if the business type is supported; otherwise, false</returns>
        protected virtual bool ValidateBizType(string bizType)
        {
            if (Options.BizTypes.Contains(bizType))
            {
                return true;
            }
            return false;
        }

    }
}
