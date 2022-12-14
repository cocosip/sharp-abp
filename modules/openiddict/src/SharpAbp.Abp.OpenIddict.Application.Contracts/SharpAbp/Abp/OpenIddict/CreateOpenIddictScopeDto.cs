using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.ObjectExtending;
using Volo.Abp.OpenIddict.Scopes;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.OpenIddict
{
    public class CreateOpenIddictScopeDto : ExtensibleObject
    {
        /// <summary>
        /// Gets or sets the public description associated with the current scope.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the display name associated with the current scope.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the unique name associated with the current scope.
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(OpenIddictScopeConsts), nameof(OpenIddictScopeConsts.NameMaxLength))]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the additional properties serialized as a JSON object,
        /// or <c>null</c> if no bag was associated with the current scope.
        /// </summary>
        public string Properties { get; set; }


        /// <summary>
        /// Gets or sets the resources associated with the
        /// current scope, serialized as a JSON array.
        /// </summary>
        public List<string> Resources { get; set; }

        /// <summary>
        /// Gets or sets the localized public descriptions associated
        /// with the current scope, serialized as a JSON object.
        /// </summary>
        public Dictionary<string, string> Descriptions { get; set; }

        /// <summary>
        /// Gets or sets the localized display names
        /// associated with the current application,
        /// serialized as a JSON object.
        /// </summary>
        public Dictionary<string, string> DisplayNames { get; set; }

        public CreateOpenIddictScopeDto()
        {
            Resources = new List<string>();
            Descriptions = new Dictionary<string, string>();
            DisplayNames = new Dictionary<string, string>();
        }
    }
}
