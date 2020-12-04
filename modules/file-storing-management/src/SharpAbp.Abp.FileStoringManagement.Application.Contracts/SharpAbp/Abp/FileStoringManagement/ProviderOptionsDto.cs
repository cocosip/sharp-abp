using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.FileStoringManagement
{

    public class ProviderOptionsDto
    {
        public string Provider { get; set; }

        public List<ProviderPropertyDto> Properties { get; set; }

        public ProviderOptionsDto(string provider)
        {
            Provider = provider;
            Properties = new List<ProviderPropertyDto>();
        }

    }

    public class ProviderPropertyDto
    {

        public string Name { get; set; }

        /// <summary>
        /// Localization value
        /// </summary>
        public string LocalizationValue { get; set; }

        /// <summary>
        /// Parameter type
        /// </summary>
        public Type Type { get; set; }

        public ProviderPropertyDto()
        {

        }

        public ProviderPropertyDto(string name, string localizationValue, Type type)
        {
            Name = name;
            LocalizationValue = localizationValue;
            Type = type;
        }
    }
}
