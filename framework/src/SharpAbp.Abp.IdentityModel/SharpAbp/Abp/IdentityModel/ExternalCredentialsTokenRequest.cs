
using IdentityModel.Client;
using System.Collections.Generic;

namespace SharpAbp.Abp.IdentityModel
{
    public class ExternalCredentialsTokenRequest : TokenRequest
    {
        /// <summary>
        /// Login provider
        /// </summary>
        public string? LoginProvider { get; set; }

        /// <summary>
        /// Provider key
        /// </summary>
        public string? ProviderKey { get; set; }

        /// <summary>
        /// Scope
        /// </summary>
        public string? Scope { get; set; }

        /// <summary>
        /// List of requested resources
        /// </summary>
        /// <value>
        /// The scope.
        /// </value>
        public ICollection<string> Resource { get; set; } = new HashSet<string>();
    }
}
