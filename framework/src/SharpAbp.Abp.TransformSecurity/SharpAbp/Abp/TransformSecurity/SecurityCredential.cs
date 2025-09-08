using System;
using Volo.Abp.ObjectExtending;

namespace SharpAbp.Abp.TransformSecurity
{
    /// <summary>
    /// Represents a security credential containing cryptographic key pair information
    /// </summary>
    [Serializable]
    public class SecurityCredential : ExtensibleObject
    {
        /// <summary>
        /// Gets or sets the unique identifier for this security credential
        /// </summary>
        public string? Identifier { get; set; }

        /// <summary>
        /// Gets or sets the key type (e.g., RSA, SM2)
        /// </summary>
        public string? KeyType { get; set; }

        /// <summary>
        /// Gets or sets the business type associated with this credential
        /// </summary>
        public string? BizType { get; set; }

        /// <summary>
        /// Gets or sets the public key in string format
        /// </summary>
        public string? PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the private key in string format
        /// </summary>
        public string? PrivateKey { get; set; }

        /// <summary>
        /// Gets or sets the expiration time of this credential
        /// </summary>
        public DateTime? Expires { get; set; }

        /// <summary>
        /// Gets or sets the creation time of this credential
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Determines whether this credential has expired at the specified date and time
        /// </summary>
        /// <param name="dateTime">The date and time to check against</param>
        /// <returns>True if the credential has expired; otherwise, false</returns>
        public bool IsExpires(DateTime dateTime)
        {
            return Expires.HasValue && Expires.Value <= dateTime;
        }
    }
}
