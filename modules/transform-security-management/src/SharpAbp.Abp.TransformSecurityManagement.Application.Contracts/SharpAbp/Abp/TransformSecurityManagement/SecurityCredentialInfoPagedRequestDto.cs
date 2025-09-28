﻿using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    /// <summary>
    /// Data Transfer Object for requesting paginated and filtered lists of security credential information.
    /// This DTO extends the standard ABP pagination functionality with domain-specific filtering capabilities
    /// for security credentials, enabling efficient querying and retrieval of credential data with various filter criteria.
    /// </summary>
    /// <remarks>
    /// This DTO inherits from PagedAndSortedResultRequestDto to provide standard pagination (skip, take)
    /// and sorting capabilities, while adding specific filter properties for security credential attributes.
    /// All filter properties are optional - when not specified, they are ignored in the query.
    /// This design supports flexible querying scenarios from simple lists to complex filtered searches.
    /// </remarks>
    public class SecurityCredentialInfoPagedRequestDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Gets or sets the identifier filter for searching security credentials by their business identifier.
        /// When specified, only credentials with matching identifiers will be returned in the result set.
        /// This filter supports exact matching for precise credential lookup operations.
        /// </summary>
        /// <value>
        /// A string representing the business identifier to filter by.
        /// If null or empty, no identifier filtering is applied and all credentials are considered.
        /// This filter is typically used when searching for a specific credential by its known business reference.
        /// </value>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the key type filter for searching security credentials by their cryptographic algorithm.
        /// This filter enables retrieval of credentials based on specific cryptographic requirements
        /// such as when an application needs only RSA or only SM2 credentials.
        /// </summary>
        /// <value>
        /// A string representing the cryptographic key type to filter by (e.g., "RSA", "SM2").
        /// If null or empty, no key type filtering is applied and credentials of all supported types are included.
        /// This filter is useful for scenarios where compatibility with specific cryptographic standards is required.
        /// </value>
        public string KeyType { get; set; }

        /// <summary>
        /// Gets or sets the business type filter for searching security credentials by their intended business use.
        /// This filter allows retrieval of credentials based on their business category or domain,
        /// enabling logical grouping and access control based on organizational requirements.
        /// </summary>
        /// <value>
        /// A string representing the business type or category to filter by.
        /// If null or empty, no business type filtering is applied and credentials from all business categories are included.
        /// Common filter values include "Payment", "Authentication", "DataEncryption", "DocumentSigning", etc.
        /// </value>
        public string BizType { get; set; }

        /// <summary>
        /// Gets or sets the minimum expiration date filter for searching credentials based on their lifecycle status.
        /// This filter enables retrieval of credentials that expire on or after the specified date,
        /// supporting scenarios such as finding credentials that are still valid or will remain valid for a certain period.
        /// </summary>
        /// <value>
        /// A nullable DateTime representing the minimum expiration date for filtering.
        /// If null, no minimum expiration date filtering is applied.
        /// When specified, only credentials with expiration dates greater than or equal to this value are included.
        /// Credentials with null expiration dates (non-expiring) are typically included in the results.
        /// </value>
        public DateTime? ExpiresMin { get; set; }

        /// <summary>
        /// Gets or sets the maximum expiration date filter for searching credentials based on their lifecycle status.
        /// This filter enables retrieval of credentials that expire before the specified date,
        /// supporting scenarios such as finding credentials that need renewal or are approaching expiration.
        /// </summary>
        /// <value>
        /// A nullable DateTime representing the maximum expiration date for filtering.
        /// If null, no maximum expiration date filtering is applied.
        /// When specified, only credentials with expiration dates less than this value are included.
        /// This filter is commonly used for proactive credential management and renewal processes.
        /// </value>
        public DateTime? ExpiresMax { get; set; }
    }
}
