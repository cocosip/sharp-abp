﻿namespace SharpAbp.Abp.CryptoVault
{
    /// <summary>
    /// Data transfer object representing an RSA key pair.
    /// This class contains the public and private key components of an RSA cryptographic key pair.
    /// </summary>
    /// <remarks>
    /// This DTO is typically used for operations that require both the public and private keys,
    /// such as key export, backup, or cryptographic operations that need the complete key pair.
    /// The keys should be in a standard format compatible with RSA cryptographic operations.
    /// </remarks>
    public class RSACredsKeyDto
    {
        /// <summary>
        /// Gets or sets the RSA public key.
        /// </summary>
        /// <value>
        /// A string representation of the RSA public key.
        /// This should be in a standard format such as PEM, DER, or XML.
        /// </value>
        /// <remarks>
        /// The public key is used for encryption and signature verification operations.
        /// It can be safely shared with other parties as it does not compromise security.
        /// The format should be compatible with the intended cryptographic operations.
        /// </remarks>
        public string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the RSA private key.
        /// </summary>
        /// <value>
        /// A string representation of the RSA private key.
        /// This should be in a standard format such as PEM, DER, or XML.
        /// </value>
        /// <remarks>
        /// The private key is used for decryption and digital signature creation.
        /// This is highly sensitive information and must be handled securely.
        /// Access to this key should be strictly controlled and logged.
        /// The format should be compatible with the intended cryptographic operations.
        /// </remarks>
        public string PrivateKey { get; set; }
    }
}
