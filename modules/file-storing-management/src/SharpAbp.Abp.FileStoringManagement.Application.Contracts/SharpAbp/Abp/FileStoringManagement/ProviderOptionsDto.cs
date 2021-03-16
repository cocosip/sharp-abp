using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.FileStoringManagement
{

    public class ProviderOptionsDto
    {
        public string Provider { get; set; }

        public List<ProviderValueDto> Values { get; set; }

        public ProviderOptionsDto(string provider)
        {
            Provider = provider;
            Values = new List<ProviderValueDto>();
        }

    }

    public class ProviderValueDto
    {

        public string Name { get; set; }

        /// <summary>
        /// Localization value
        /// </summary>
        public string LocalizationValue { get; set; }

        /// <summary>
        /// Parameter type
        /// </summary>
        public string Type { get; set; }

        public string Note { get; set; }

        public ProviderValueDto()
        {

        }

        public ProviderValueDto(string name, string localizationValue, string type, string note)
        {
            Name = name;
            LocalizationValue = localizationValue;
            Type = type;
            Note = note;
        }
    }
}
