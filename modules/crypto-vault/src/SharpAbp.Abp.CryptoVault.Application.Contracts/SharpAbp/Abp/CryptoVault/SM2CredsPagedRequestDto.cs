using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.CryptoVault
{
    /// <summary>
    /// Data transfer object for paginated and filtered SM2 credentials requests.
    /// This class extends the base paging functionality with SM2-specific filtering capabilities.
    /// </summary>
    public class SM2CredsPagedRequestDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Gets or sets the unique identifier filter for SM2 credentials.
        /// </summary>
        /// <value>
        /// A string value used to filter SM2 credentials by their identifier.
        /// If null or empty, no identifier filtering is applied.
        /// </value>
        /// <remarks>
        /// This property supports partial matching and is case-insensitive.
        /// It can be used to search for specific credentials by their business identifier.
        /// </remarks>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the source type filter for SM2 credentials.
        /// </summary>
        /// <value>
        /// An optional integer value representing the source type of the credentials.
        /// If null, no source type filtering is applied.
        /// </value>
        /// <remarks>
        /// The source type indicates how the credentials were created or imported.
        /// Different values may represent different generation methods or import sources.
        /// </remarks>
        public int? SourceType { get; set; }

        /// <summary>
        /// Gets or sets the elliptic curve filter for SM2 credentials.
        /// </summary>
        /// <value>
        /// A string value representing the elliptic curve name used for filtering.
        /// If null or empty, no curve filtering is applied.
        /// </value>
        /// <remarks>
        /// Common curve values include "sm2p256v1" for the standard SM2 curve.
        /// This filter allows querying credentials generated with specific curve parameters.
        /// </remarks>
        public string Curve { get; set; }
    }
}
