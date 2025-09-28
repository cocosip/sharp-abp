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
    /// <summary>
    /// Database-backed implementation of the security credential manager.
    /// This class manages the generation and retrieval of security credentials for RSA and SM2 encryption algorithms,
    /// storing and retrieving cryptographic keys from a database repository.
    /// </summary>
    [Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
    [ExposeServices(typeof(ISecurityCredentialManager))]
    public class DatabaseSecurityCredentialManager : ISecurityCredentialManager, ITransientDependency
    {
        /// <summary>
        /// Gets the transform security options containing global configuration settings.
        /// </summary>
        protected AbpTransformSecurityOptions Options { get; }
        
        /// <summary>
        /// Gets the RSA-specific configuration options including key size and padding settings.
        /// </summary>
        protected AbpTransformSecurityRSAOptions RSAOptions { get; }
        
        /// <summary>
        /// Gets the SM2-specific configuration options including curve and mode settings.
        /// </summary>
        protected AbpTransformSecuritySM2Options SM2Options { get; }
        
        /// <summary>
        /// Gets the GUID generator for creating unique identifiers for security credentials.
        /// </summary>
        protected IGuidGenerator GuidGenerator { get; }
        
        /// <summary>
        /// Gets the clock service for managing time-related operations and credential expiration.
        /// </summary>
        protected IClock Clock { get; }
        
        /// <summary>
        /// Gets the key service for encrypting and decrypting cryptographic keys.
        /// </summary>
        protected IKeyService KeyService { get; }
        
        /// <summary>
        /// Gets the security credential store for persisting and retrieving credentials.
        /// </summary>
        protected ISecurityCredentialStore SecurityCredentialStore { get; }
        
        /// <summary>
        /// Gets the repository for managing RSA credentials in the database.
        /// </summary>
        protected IRSACredsRepository RSACredsRepository { get; }
        
        /// <summary>
        /// Gets the repository for managing SM2 credentials in the database.
        /// </summary>
        protected ISM2CredsRepository SM2CredsRepository { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseSecurityCredentialManager"/> class.
        /// </summary>
        /// <param name="options">The transform security options.</param>
        /// <param name="rsaOptions">The RSA-specific configuration options.</param>
        /// <param name="sm2Options">The SM2-specific configuration options.</param>
        /// <param name="guidGenerator">The GUID generator service.</param>
        /// <param name="clock">The clock service for time operations.</param>
        /// <param name="keyService">The key service for cryptographic operations.</param>
        /// <param name="securityCredentialStore">The security credential store.</param>
        /// <param name="rsaCredsRepository">The RSA credentials repository.</param>
        /// <param name="sm2CredsRepository">The SM2 credentials repository.</param>
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

        /// <summary>
        /// Generates a new security credential for the specified business type.
        /// This method creates appropriate credentials based on the configured encryption algorithm (RSA or SM2),
        /// retrieves the corresponding cryptographic keys from the database, and stores the credential.
        /// </summary>
        /// <param name="bizType">The business type identifier for which the credential is being generated.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A newly generated security credential with cryptographic keys and metadata.</returns>
        /// <exception cref="AbpException">Thrown when the business type is not supported, encryption algorithm is unsupported, or credential retrieval fails.</exception>
        public virtual async Task<SecurityCredential> GenerateAsync(string bizType, CancellationToken cancellationToken = default)
        {
            if (!ValidateBizType(bizType))
            {
                throw new AbpException($"Unsupported bizType '{bizType}'. Please check the configuration to ensure this bizType is supported.");
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
                var rsaCreds = await RSACredsRepository.GetRandomAsync(size: credential.GetRSAKeySize(), cancellationToken: cancellationToken) ?? throw new AbpException("Failed to retrieve RSA credentials. Please ensure RSA credentials are properly configured and available.");

                credential.SetReferenceId(rsaCreds.Id.ToString("N"));
                credential.SetSM2Curve(SM2Options.Curve);
                credential.SetSM2Mode(SM2Options.Mode);
                credential.PublicKey = KeyService.DecryptKey(rsaCreds.PublicKey, rsaCreds.PassPhrase, rsaCreds.Salt);
                credential.PrivateKey = KeyService.DecryptKey(rsaCreds.PrivateKey, rsaCreds.PassPhrase, rsaCreds.Salt);

            }
            else if (Options.EncryptionAlgo.Equals("SM2", StringComparison.OrdinalIgnoreCase))
            {
                credential.KeyType = "SM2";
                var sm2Creds = await SM2CredsRepository.GetRandomAsync(curve: credential.GetSM2Curve(), cancellationToken: cancellationToken) ?? throw new AbpException("Failed to retrieve SM2 credentials. Please ensure SM2 credentials are properly configured and available.");

                credential.SetReferenceId(sm2Creds.Id.ToString("N"));
                credential.SetRSAKeySize(RSAOptions.KeySize);
                credential.SetRSAPadding(RSAOptions.Padding);
                credential.PublicKey = KeyService.DecryptKey(sm2Creds.PublicKey, sm2Creds.PassPhrase, sm2Creds.Salt);
                credential.PrivateKey = KeyService.DecryptKey(sm2Creds.PrivateKey, sm2Creds.PassPhrase, sm2Creds.Salt);
            }
            else
            {
                throw new AbpException($"Unsupported encryption algorithm '{Options.EncryptionAlgo}'. Only 'RSA' and 'SM2' encryption algorithms are supported.");
            }
            await SecurityCredentialStore.SetAsync(credential);
            return credential;
        }

        /// <summary>
        /// Validates whether the specified business type is supported according to the current configuration.
        /// </summary>
        /// <param name="bizType">The business type to validate.</param>
        /// <returns>True if the business type is supported; otherwise, false.</returns>
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