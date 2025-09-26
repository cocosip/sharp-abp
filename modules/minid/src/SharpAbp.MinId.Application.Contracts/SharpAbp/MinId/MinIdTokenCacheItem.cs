﻿namespace SharpAbp.MinId
{
    /// <summary>
    /// Cache item for MinId token authentication.
    /// Used to store token validation information in distributed cache for efficient authentication.
    /// This class contains the minimal information needed to validate MinId tokens without database access.
    /// </summary>
    public class MinIdTokenCacheItem
    {
        /// <summary>
        /// Gets or sets the business type identifier.
        /// Used to associate the token with a specific business context or application.
        /// Must be between 3 and 32 alphanumeric characters.
        /// </summary>
        public string BizType { get; set; }

        /// <summary>
        /// Gets or sets the authentication token value.
        /// Used to validate requests for ID generation services.
        /// Must be between 3 and 40 alphanumeric characters.
        /// </summary>
        public string Token { get; set; }
    }
}