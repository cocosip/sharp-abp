namespace SharpAbp.Abp.AspNetCore.Http
{
    /// <summary>
    /// Provides options for configuring HTTP headers in the ABP framework.
    /// </summary>
    public class AbpHttpHeadersOptions
    { 
        /// <summary>
        /// Gets or sets the prefix used for route translation headers.
        /// This prefix is used to identify headers that contain route translation information.
        /// </summary>
        /// <value>
        /// The prefix for route translation headers. The default value is null.
        /// </value>
        public string? RouteTranslationPrefix { get; set; }
    }
}