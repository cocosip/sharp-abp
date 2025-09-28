using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.CryptoVault
{
    /// <summary>
    /// Data transfer object representing SM2 cryptographic credentials.
    /// This class contains all the information about an SM2 key pair stored in the crypto vault.
    /// </summary>
    public class SM2CredsDto : AuditedEntityDto<Guid>
    {
        /// <summary>
        /// Gets or sets the unique business identifier for the SM2 credentials.
        /// </summary>
        /// <value>
        /// A string that uniquely identifies these credentials within the business context.
        /// This is different from the technical entity ID and is used for business operations.
        /// </value>
        /// <remarks>
        /// This identifier is typically user-friendly and can be used for searching and referencing
        /// the credentials in business operations and user interfaces.
        /// </remarks>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the elliptic curve name used for the SM2 key pair.
        /// </summary>
        /// <value>
        /// A string representing the elliptic curve name. Default value is "sm2p256v1".
        /// Supported curves include "wapip192v1" and "sm2p256v1".
        /// </value>
        /// <remarks>
        /// The curve name defines the mathematical parameters for the elliptic curve
        /// used in the SM2 cryptographic algorithm. Different curves provide different
        /// security levels and performance characteristics.
        /// "sm2p256v1" is the standard curve recommended for SM2 operations.
        /// </remarks>
        public string Curve { get; set; }

        /// <summary>
        /// Gets or sets the encrypted SM2 public key.
        /// </summary>
        /// <value>
        /// A string containing the SM2 public key in encrypted form.
        /// The actual format depends on the encryption method used by the crypto vault.
        /// </value>
        /// <remarks>
        /// The public key is stored in encrypted form for additional security.
        /// It must be decrypted before use in cryptographic operations.
        /// The public key is used for encryption and signature verification in SM2 operations.
        /// </remarks>
        public string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the encrypted SM2 private key.
        /// </summary>
        /// <value>
        /// A string containing the SM2 private key in encrypted form.
        /// The actual format depends on the encryption method used by the crypto vault.
        /// </value>
        /// <remarks>
        /// The private key is stored in encrypted form for security purposes.
        /// It must be decrypted before use in cryptographic operations.
        /// The private key is used for decryption and digital signature creation in SM2 operations.
        /// This is highly sensitive information and should be handled with extreme care.
        /// </remarks>
        public string PrivateKey { get; set; }

        /// <summary>
        /// Gets or sets the description of the SM2 credentials.
        /// </summary>
        /// <value>
        /// A string containing a human-readable description of the credentials.
        /// This can include information about the purpose, usage, or any other relevant details.
        /// </value>
        /// <remarks>
        /// The description is optional and is intended to provide additional context
        /// about the credentials for administrators and users.
        /// </remarks>
        public string Description { get; set; }
    }
}
