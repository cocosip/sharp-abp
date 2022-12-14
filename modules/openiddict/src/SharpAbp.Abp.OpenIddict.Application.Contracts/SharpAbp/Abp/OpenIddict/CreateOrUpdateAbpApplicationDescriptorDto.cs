using System.Collections.Generic;

namespace SharpAbp.Abp.OpenIddict
{
    public class CreateOrUpdateAbpApplicationDescriptorDto
    {
        /// <summary>
        /// Gets or sets the client identifier associated with the current application.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret associated with the current application.
        /// Note: depending on the application manager used to create this instance,
        /// this property may be hashed or encrypted for security reasons.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the consent type associated with the current application.
        /// </summary>
        public string ConsentType { get; set; }

        /// <summary>
        /// Gets or sets the display name associated with the current application.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the localized display names
        /// associated with the current application,
        /// serialized as a JSON object.
        /// </summary>
        public List<string> DisplayNames { get; set; }

        /// <summary>
        /// Gets or sets the permissions associated with the
        /// current application, serialized as a JSON array.
        /// </summary>
        public List<string> Permissions { get; set; }

        /// <summary>
        /// Gets or sets the logout callback URLs associated with
        /// the current application, serialized as a JSON array.
        /// </summary>
        public List<string> PostLogoutRedirectUris { get; set; }

        ///// <summary>
        ///// Gets or sets the additional properties serialized as a JSON object,
        ///// or <c>null</c> if no bag was associated with the current application.
        ///// </summary>
        //public string Properties { get; set; }

        /// <summary>
        /// Gets or sets the callback URLs associated with the
        /// current application, serialized as a JSON array.
        /// </summary>
        public List<string> RedirectUris { get; set; }

        /// <summary>
        /// Gets or sets the requirements associated with the
        /// current application, serialized as a JSON array.
        /// </summary>
        public List<string> Requirements { get; set; }

        /// <summary>
        /// Gets or sets the application type associated with the current application.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// URI to further information about client.
        /// </summary>
        public string ClientUri { get; set; }

        /// <summary>
        /// URI to client logo.
        /// </summary>
        public string LogoUri { get; set; }

        public CreateOrUpdateAbpApplicationDescriptorDto()
        {
            DisplayNames = new List<string>();
            Permissions = new List<string>();
            PostLogoutRedirectUris = new List<string>();
            RedirectUris = new List<string>();
            Requirements = new List<string>();
        }
    }
}
