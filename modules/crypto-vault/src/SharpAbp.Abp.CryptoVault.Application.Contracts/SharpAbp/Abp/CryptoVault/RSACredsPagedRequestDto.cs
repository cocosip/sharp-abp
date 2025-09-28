using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.CryptoVault
{
    /// <summary>
    /// Data transfer object for paginated and filtered RSA credentials requests.
    /// This class extends the base paging functionality with RSA-specific filtering capabilities.
    /// </summary>
    public class RSACredsPagedRequestDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Gets or sets the unique identifier filter for RSA credentials.
        /// </summary>
        /// <value>
        /// A string value used to filter RSA credentials by their identifier.
        /// If null or empty, no identifier filtering is applied.
        /// </value>
        /// <remarks>
        /// This property supports partial matching and is case-insensitive.
        /// It can be used to search for specific credentials by their business identifier.
        /// </remarks>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the source type filter for RSA credentials.
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
        /// Gets or sets the RSA key size filter for credentials.
        /// </summary>
        /// <value>
        /// An optional integer value representing the RSA key size in bits.
        /// If null, no key size filtering is applied.
        /// </value>
        /// <remarks>
        /// Common RSA key sizes include 1024, 2048, 3072, and 4096 bits.
        /// This filter allows querying credentials with specific key strengths.
        /// </remarks>
        public int? Size { get; set; }
    }
}
