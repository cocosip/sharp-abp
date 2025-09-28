﻿using System.Collections.Generic;

namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Data transfer object that represents file storing provider options and their available configuration values.
    /// </summary>
    public class ProviderOptionsDto
    {
        /// <summary>
        /// Gets or sets the name of the file storing provider.
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// Gets or sets the list of configuration values available for this provider.
        /// </summary>
        public List<ProviderValueDto> Values { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderOptionsDto"/> class with the specified provider name.
        /// </summary>
        /// <param name="provider">The name of the file storing provider.</param>
        public ProviderOptionsDto(string provider)
        {
            Provider = provider;
            Values = [];
        }

    }

    /// <summary>
    /// Data transfer object that represents a configuration value for a file storing provider.
    /// </summary>
    public class ProviderValueDto
    {
        /// <summary>
        /// Gets or sets the name of the configuration parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the localized display value for the configuration parameter.
        /// </summary>
        public string LocalizationValue { get; set; }

        /// <summary>
        /// Gets or sets the data type of the configuration parameter (e.g., "string", "int", "bool").
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets an example value for the configuration parameter.
        /// </summary>
        public string Eg { get; set; }

        /// <summary>
        /// Gets or sets additional notes or description for the configuration parameter.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderValueDto"/> class.
        /// </summary>
        public ProviderValueDto()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderValueDto"/> class with the specified parameters.
        /// </summary>
        /// <param name="name">The name of the configuration parameter.</param>
        /// <param name="localizationValue">The localized display value.</param>
        /// <param name="type">The data type of the parameter.</param>
        /// <param name="eg">An example value for the parameter.</param>
        /// <param name="note">Additional notes or description.</param>
        public ProviderValueDto(string name, string localizationValue, string type, string eg, string note)
        {
            Name = name;
            LocalizationValue = localizationValue;
            Type = type;
            Eg = eg;
            Note = note;
        }
    }
}
