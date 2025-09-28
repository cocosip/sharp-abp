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
    /// <summary>
    /// Database-backed implementation of the security credential store.
    /// This class provides persistent storage and retrieval of security credentials using database repositories,
    /// supporting both RSA and SM2 encryption algorithms with encrypted key storage.
    /// </summary>
    [Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
    [ExposeServices(typeof(ISecurityCredentialStore))]
    public class DatabaseSecurityCredentialStore : ISecurityCredentialStore, ITransientDependency
    {
        /// <summary>
        /// Gets the GUID generator for creating unique identifiers.
        /// </summary>
        protected IGuidGenerator GuidGenerator { get; }
        
        /// <summary>
        /// Gets the key service for encrypting and decrypting cryptographic keys.
        /// </summary>
        protected IKeyService KeyService { get; }
        
        /// <summary>
        /// Gets the repository for managing security credential information in the database.
        /// </summary>
        protected ISecurityCredentialInfoRepository SecurityCredentialInfoRepository { get; }
        
        /// <summary>
        /// Gets the repository for managing RSA credentials in the database.
        /// </summary>
        protected IRSACredsRepository RSACredsRepository { get; }
        
        /// <summary>
        /// Gets the repository for managing SM2 credentials in the database.
        /// </summary>
        protected ISM2CredsRepository SM2CredsRepository { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseSecurityCredentialStore"/> class.
        /// </summary>
        /// <param name="guidGenerator">The GUID generator service.</param>
        /// <param name="keyService">The key service for cryptographic operations.</param>
        /// <param name="securityCredentialInfoRepository">The security credential information repository.</param>
        /// <param name="rsaCredsRepository">The RSA credentials repository.</param>
        /// <param name="sm2CredsRepository">The SM2 credentials repository.</param>
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

        /// <summary>
        /// Retrieves a security credential by its unique identifier.
        /// This method loads the credential information from the database and decrypts the associated
        /// cryptographic keys based on the credential's key type (RSA or SM2).
        /// </summary>
        /// <param name="identifier">The unique identifier of the security credential to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>The security credential with decrypted cryptographic keys and metadata.</returns>
        /// <exception cref="AbpException">Thrown when the credential with the specified identifier is not found or when an unsupported key type is encountered.</exception>
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

        /// <summary>
        /// Stores a security credential in the database.
        /// This method persists the credential's metadata and reference information to the database,
        /// linking it to the appropriate cryptographic key store (RSA or SM2).
        /// </summary>
        /// <param name="credential">The security credential to store.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous store operation.</returns>
        /// <exception cref="AbpException">Thrown when the credential's reference identifier is null or empty.</exception>
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