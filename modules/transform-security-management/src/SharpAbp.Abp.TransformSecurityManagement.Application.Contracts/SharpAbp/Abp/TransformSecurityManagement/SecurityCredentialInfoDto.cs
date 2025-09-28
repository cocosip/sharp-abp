﻿using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    /// <summary>
    /// Data Transfer Object representing security credential information for API responses and client consumption.
    /// This DTO provides a complete view of a security credential entity including all business and technical metadata,
    /// audit information, and lifecycle details. It serves as the primary data contract for security credential
    /// operations in the Transform Security Management system.
    /// </summary>
    /// <remarks>
    /// This DTO inherits from AuditedEntityDto to provide comprehensive audit trail information
    /// including creation time, modification time, and user tracking. It contains all non-sensitive
    /// information about a security credential while ensuring that actual cryptographic material
    /// remains securely stored in the CryptoVault system.
    /// </remarks>
    public class SecurityCredentialInfoDto : AuditedEntityDto<Guid>
    {
        /// <summary>
        /// Gets or sets the unique business identifier for the security credential.
        /// This identifier serves as a human-readable reference that can be used in configurations,
        /// API calls, and business processes to locate and reference the credential across different systems.
        /// </summary>
        /// <value>
        /// A string value that uniquely identifies the security credential within the business domain.
        /// This identifier is typically more meaningful than the technical GUID and is used in external integrations.
        /// </value>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the corresponding cryptographic key stored in the CryptoVault system.
        /// This property establishes the link between the credential metadata and the actual cryptographic material,
        /// enabling secure separation of sensitive key data from business information.
        /// </summary>
        /// <value>
        /// A GUID that references the cryptographic key entry in the CryptoVault system.
        /// This ID is used internally to retrieve the actual RSA or SM2 key material when performing cryptographic operations.
        /// </value>
        public Guid CredsId { get; set; }

        /// <summary>
        /// Gets or sets the type of cryptographic algorithm used by this credential.
        /// This property indicates the cryptographic family and determines which operations
        /// and protocols can be used with the associated key material.
        /// </summary>
        /// <value>
        /// A string indicating the cryptographic key type. Supported values include:
        /// - "RSA": RSA public-key cryptography algorithm for encryption, decryption, and digital signatures
        /// - "SM2": Chinese national cryptographic standard based on elliptic curve cryptography
        /// Additional algorithms may be supported based on system configuration and security policies.
        /// </value>
        public string KeyType { get; set; }

        /// <summary>
        /// Gets or sets the business type or category that classifies the intended use of this credential.
        /// This property enables business-level organization, access control, and policy application
        /// based on the credential's intended purpose within the organization's security framework.
        /// </summary>
        /// <value>
        /// A string representing the business category or use case for the credential.
        /// This value is used for filtering, access control, compliance reporting, and operational management.
        /// Common categories include payment processing, user authentication, data encryption, and document signing.
        /// </value>
        public string BizType { get; set; }

        /// <summary>
        /// Gets or sets the expiration date and time for the cryptographic credential.
        /// This property supports key lifecycle management and compliance with security policies
        /// that require periodic key rotation. Expired credentials may still be used for legacy data decryption.
        /// </summary>
        /// <value>
        /// A nullable DateTime indicating when the credential expires and should no longer be used for new operations.
        /// If null, the credential does not have an expiration date and remains valid until explicitly revoked.
        /// Expiration dates are checked during cryptographic operations to ensure compliance with security policies.
        /// </value>
        public DateTime? Expires { get; set; }

        /// <summary>
        /// Gets or sets additional descriptive information about the security credential.
        /// This property provides context, usage guidelines, restrictions, or other relevant information
        /// that helps administrators and developers understand the credential's purpose and proper usage.
        /// </summary>
        /// <value>
        /// A string containing human-readable description text about the credential.
        /// This field is optional and may include information such as the credential's purpose,
        /// usage restrictions, associated applications, or administrative notes.
        /// </value>
        public string Description { get; set; }
    }
}
