using System;

namespace SharpAbp.Abp.FileStoring
{
    public class FileProviderConfigurationItem
    {
        /// <summary>
        /// The type of value
        /// </summary>
        public Type? ValueType { get; set; }

        /// <summary>
        /// Example
        /// </summary>
        public string? Eg { get; set; }

        /// <summary>
        /// Note Localization name
        /// </summary>
        public string? NoteLocalizationName { get; set; }

        public FileProviderConfigurationItem()
        {

        }

        public FileProviderConfigurationItem(Type valueType, string eg, string noteLocalizationName)
        {
            ValueType = valueType;
            Eg = eg;
            NoteLocalizationName = noteLocalizationName;
        }
    }
}