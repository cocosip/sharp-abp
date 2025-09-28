﻿namespace SharpAbp.Abp.CryptoVault
{
    /// <summary>
    /// Data transfer object representing an SM2 key pair.
    /// This class contains the public and private key components of an SM2 cryptographic key pair.
    /// </summary>
    /// <remarks>
    /// This DTO is typically used for operations that require both the public and private keys,
    /// such as key export, backup, or cryptographic operations that need the complete key pair.
    /// The keys should be in a standard format compatible with SM2 cryptographic operations.
    /// SM2 is a public key cryptographic algorithm based on elliptic curves.
    /// </remarks>
    public class SM2CredsKeyDto
    {
        /// <summary>
        /// Gets or sets the SM2 public key.
        /// </summary>
        /// <value>
        /// A string representation of the SM2 public key.
        /// This should be in a standard format such as PEM, DER, or hexadecimal string.
        /// </value>
        /// <remarks>
        /// The public key is used for encryption and signature verification operations in SM2 algorithm.
        /// It can be safely shared with other parties as it does not compromise security.
        /// The format should be compatible with the intended SM2 cryptographic operations.
        /// </remarks>
        public string PublicKey { get; set; }
        
        /// <summary>
        /// Gets or sets the SM2 private key.
        /// </summary>
        /// <value>
        /// A string representation of the SM2 private key.
        /// This should be in a standard format such as PEM, DER, or hexadecimal string.
        /// </value>
        /// <remarks>
        /// The private key is used for decryption and digital signature creation in SM2 algorithm.
        /// This is highly sensitive information and must be handled securely.
        /// Access to this key should be strictly controlled and logged.
        /// The format should be compatible with the intended SM2 cryptographic operations.
        /// </remarks>
        public string PrivateKey { get; set; }
    }
}
