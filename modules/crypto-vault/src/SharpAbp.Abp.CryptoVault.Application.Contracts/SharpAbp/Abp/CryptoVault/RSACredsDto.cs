﻿using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.CryptoVault
{
    /// <summary>
    /// Data transfer object representing RSA cryptographic credentials.
    /// This class contains all the information about an RSA key pair stored in the crypto vault.
    /// </summary>
    public class RSACredsDto : AuditedEntityDto<Guid>
    {
        /// <summary>
        /// Gets or sets the unique business identifier for the RSA credentials.
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
        /// Gets or sets the RSA key size in bits.
        /// </summary>
        /// <value>
        /// An integer representing the size of the RSA key in bits.
        /// Common values are 1024, 2048, 3072, and 4096.
        /// </value>
        /// <remarks>
        /// The key size determines the cryptographic strength of the RSA key pair.
        /// Larger key sizes provide better security but require more computational resources.
        /// Keys smaller than 2048 bits are generally not recommended for new applications.
        /// </remarks>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the encrypted RSA public key.
        /// </summary>
        /// <value>
        /// A string containing the RSA public key in encrypted form.
        /// The actual format depends on the encryption method used by the crypto vault.
        /// </value>
        /// <remarks>
        /// The public key is stored in encrypted form for additional security.
        /// It must be decrypted before use in cryptographic operations.
        /// The public key is used for encryption and signature verification.
        /// </remarks>
        public string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the encrypted RSA private key.
        /// </summary>
        /// <value>
        /// A string containing the RSA private key in encrypted form.
        /// The actual format depends on the encryption method used by the crypto vault.
        /// </value>
        /// <remarks>
        /// The private key is stored in encrypted form for security purposes.
        /// It must be decrypted before use in cryptographic operations.
        /// The private key is used for decryption and digital signature creation.
        /// This is highly sensitive information and should be handled with extreme care.
        /// </remarks>
        public string PrivateKey { get; set; }

        /// <summary>
        /// Gets or sets the description of the RSA credentials.
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
