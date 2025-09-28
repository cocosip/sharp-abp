using System.ComponentModel.DataAnnotations;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    /// <summary>
    /// Data Transfer Object for creating a new security credential information entity.
    /// This DTO contains the minimal required information needed to establish a new security credential
    /// in the Transform Security Management system. Additional properties like identifier and credentials ID
    /// are typically generated automatically during the creation process.
    /// </summary>
    /// <remarks>
    /// This DTO is designed to capture only the essential business information required from the user
    /// to create a security credential. The system will handle the generation of technical identifiers,
    /// cryptographic key creation, and other internal processes automatically.
    /// </remarks>
    public class CreateSecurityCredentialInfoDto
    {
        /// <summary>
        /// Gets or sets the business type or category for the security credential being created.
        /// This property is required and determines how the credential will be categorized and used
        /// within the business domain. It enables logical grouping and access control based on business requirements.
        /// </summary>
        /// <value>
        /// A string representing the business type or use case category.
        /// Examples might include "Payment", "Authentication", "DataEncryption", "DocumentSigning", etc.
        /// This value must not be null or empty as it's essential for proper credential management.
        /// </value>
        /// <example>
        /// Common business types:
        /// - "Payment": For payment processing and financial transactions
        /// - "Authentication": For user authentication and authorization
        /// - "DataEncryption": For general data encryption purposes
        /// - "DocumentSigning": For digital document signing operations
        /// </example>
        [Required]
        public string BizType { get; set; }
    }
}
