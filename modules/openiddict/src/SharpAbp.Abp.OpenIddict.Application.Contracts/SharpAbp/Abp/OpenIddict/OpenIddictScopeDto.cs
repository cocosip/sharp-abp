﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.OpenIddict
{
    public class OpenIddictScopeDto : ExtensibleEntityDto<Guid>
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
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the additional properties serialized as a JSON object,
        /// or <c>null</c> if no bag was associated with the current scope.
        /// </summary>
        public Dictionary<string, JsonElement> Properties { get; set; }

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

        public OpenIddictScopeDto()
        {
            Properties = new Dictionary<string, JsonElement>();
            Resources = new List<string>();
            Descriptions = new Dictionary<string, string>();
            DisplayNames = new Dictionary<string, string>();
        }
    }
}
