namespace SharpAbp.Abp.TransformSecurity.AspNetCore
{
    /// <summary>
    /// Data transfer object for security credential public key information
    /// </summary>
    public class SecurityCredentialPublicKeyDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the security credential
        /// </summary>
        public string? Identifier { get; set; }

        /// <summary>
        /// Gets or sets the business type associated with the credential
        /// </summary>
        public string? BizType { get; set; }

        /// <summary>
        /// Gets or sets the key type (RSA, SM2)
        /// </summary>
        public string? KeyType { get; set; }

        /// <summary>
        /// Gets or sets the public key content
        /// </summary>
        public string? PublicKey { get; set; }

    }
}
