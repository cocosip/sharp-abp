using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.OpenIddict
{
    public class OpenIddictApplicationDto : ExtensibleFullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// Gets or sets the client identifier associated with the current application.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the application type associated with the current application.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the consent type associated with the current application.
        /// </summary>
        public string ConsentType { get; set; }

        /// <summary>
        /// Gets or sets the display name associated with the current application.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the client secret associated with the current application.
        /// Note: depending on the application manager used to create this instance,
        /// this property may be hashed or encrypted for security reasons.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// URI to further information about client.
        /// </summary>
        public string ClientUri { get; set; }

        /// <summary>
        /// URI to client logo.
        /// </summary>
        public string LogoUri { get; set; }

        /// <summary>
        /// Gets or sets the additional properties serialized as a JSON object,
        /// or <c>null</c> if no bag was associated with the current scope.
        /// </summary>
        public string Properties { get; set; }

        /// <summary>
        /// DisplayNames
        /// </summary>
        public Dictionary<string, string> DisplayNames { get; set; }

        /// <summary>
        /// RedirectUris
        /// </summary>
        public List<string> RedirectUris { get; set; }

        /// <summary>
        /// Gets or sets the logout callback URLs associated with
        /// the current application, serialized as a JSON array.
        /// </summary>
        public List<string> PostLogoutRedirectUris { get; set; }

        /// <summary>
        /// Gets or sets the requirements associated with the
        /// current application, serialized as a JSON array.
        /// </summary>
        public List<string> Requirements { get; set; }

        /// <summary>
        /// GrantTypes
        /// </summary>
        public List<string> GrantTypes { get; set; }

        /// <summary>
        /// Scopes
        /// </summary>
        public List<string> Scopes { get; set; }

        public OpenIddictApplicationDto()
        {
            DisplayNames = new Dictionary<string, string>();
            RedirectUris = new List<string>();
            PostLogoutRedirectUris = new List<string>();
            Requirements = new List<string>();

            GrantTypes = new List<string>();
            Scopes = new List<string>();
        }
    }
}
