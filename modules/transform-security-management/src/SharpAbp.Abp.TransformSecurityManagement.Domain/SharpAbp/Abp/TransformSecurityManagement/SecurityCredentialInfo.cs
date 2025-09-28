using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    /// <summary>
    /// Represents a security credential information entity that manages cryptographic credentials
    /// for secure data transformation and encryption operations.
    /// This aggregate root provides a centralized management approach for various types of security credentials
    /// including RSA and SM2 cryptographic keys with their associated metadata and lifecycle information.
    /// </summary>
    /// <remarks>
    /// This entity serves as a bridge between the Transform Security Management domain and the CryptoVault system,
    /// enabling secure storage and retrieval of cryptographic materials with proper audit trail and expiration management.
    /// The entity supports multi-tenancy and provides comprehensive auditing capabilities through the AuditedAggregateRoot base class.
    /// </remarks>
    public class SecurityCredentialInfo : AuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier for the security credential.
        /// This identifier serves as a business key for locating and referencing the credential
        /// across different operations and systems. Must be unique within the tenant scope.
        /// </summary>
        /// <value>
        /// A string value that uniquely identifies the security credential.
        /// This value is typically used in API calls and configuration references.
        /// </value>
        public virtual string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the corresponding cryptographic key stored in the CryptoVault system.
        /// This property establishes a relationship with the actual cryptographic material
        /// stored securely in the vault, enabling separation of metadata and sensitive key data.
        /// </summary>
        /// <value>
        /// A GUID that references the cryptographic key entry in the CryptoVault system.
        /// This ID is used to retrieve the actual RSA or SM2 key material when needed for cryptographic operations.
        /// </value>
        public virtual Guid CredsId { get; set; }

        /// <summary>
        /// Gets or sets the cryptographic key type supported by this credential.
        /// Specifies the algorithm family used for the cryptographic operations.
        /// </summary>
        /// <value>
        /// A string indicating the key type. Common values include:
        /// - "RSA": RSA public-key cryptography algorithm
        /// - "SM2": Chinese national cryptographic algorithm (elliptic curve based)
        /// Additional key types may be supported based on system configuration.
        /// </value>
        public virtual string KeyType { get; set; }

        /// <summary>
        /// Gets or sets the business type or category that this credential is associated with.
        /// This property enables logical grouping and filtering of credentials based on their intended use case
        /// or business domain within the application ecosystem.
        /// </summary>
        /// <value>
        /// A string value representing the business category or use case.
        /// Examples might include "Payment", "Authentication", "DataEncryption", etc.
        /// This value is used for filtering, reporting, and access control purposes.
        /// </value>
        public virtual string BizType { get; set; }

        /// <summary>
        /// Gets or sets the expiration date and time for the cryptographic key.
        /// When set, this property indicates when the credential should no longer be used for new operations.
        /// Expired credentials may still be used for decryption of previously encrypted data.
        /// </summary>
        /// <value>
        /// A nullable DateTime indicating when the credential expires.
        /// If null, the credential does not have an expiration date and remains valid indefinitely.
        /// The expiration is typically checked during cryptographic operations to ensure key validity.
        /// </value>
        public virtual DateTime? Expires { get; set; }

        /// <summary>
        /// Gets or sets additional descriptive information about the security credential.
        /// This property provides a human-readable description that can include usage notes,
        /// purpose, restrictions, or any other relevant information for administrators and developers.
        /// </summary>
        /// <value>
        /// A string containing descriptive text about the credential.
        /// This field is optional and primarily used for documentation and administrative purposes.
        /// </value>
        public virtual string Description { get; set; }

        /// <summary>
        /// Initializes a new instance of the SecurityCredentialInfo class with default values.
        /// This parameterless constructor is required for Entity Framework Core and serialization scenarios.
        /// </summary>
        public SecurityCredentialInfo()
        {

        }

        /// <summary>
        /// Initializes a new instance of the SecurityCredentialInfo class with the specified identifier.
        /// This constructor is typically used when creating a new credential entity with a pre-generated ID.
        /// </summary>
        /// <param name="id">The unique identifier for the security credential entity.</param>
        public SecurityCredentialInfo(Guid id) : base(id)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SecurityCredentialInfo class with all required properties.
        /// This constructor provides a convenient way to create a fully populated credential entity
        /// with all necessary information for immediate use in the system.
        /// </summary>
        /// <param name="id">The unique identifier for the security credential entity.</param>
        /// <param name="identifier">The business identifier for the credential.</param>
        /// <param name="credsId">The identifier of the corresponding cryptographic key in the CryptoVault system.</param>
        /// <param name="keyType">The type of cryptographic key (e.g., "RSA", "SM2").</param>
        /// <param name="bizType">The business type or category for this credential.</param>
        /// <param name="expires">The optional expiration date for the credential.</param>
        /// <param name="description">Optional descriptive information about the credential.</param>
        public SecurityCredentialInfo(
            Guid id,
            string identifier,
            Guid credsId,
            string keyType,
            string bizType,
            DateTime? expires,
            string description) : base(id)
        {
            Identifier = identifier;
            CredsId = credsId;
            KeyType = keyType;
            BizType = bizType;
            Expires = expires;
            Description = description;
        }
    }
}