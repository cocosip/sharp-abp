using System;
using System.ComponentModel;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Represents a configuration item for a file provider.
    /// </summary>
    public class FileProviderConfigurationItem
    {
        /// <summary>
        /// Gets or sets the type of the configuration value.
        /// </summary>
        public Type? ValueType { get; set; }

        /// <summary>
        /// Gets or sets an example value for the configuration item.
        /// </summary>
        public string? Eg { get; set; }

        /// <summary>
        /// Gets or sets the localization name for notes or descriptions related to this configuration item.
        /// </summary>
        public string? NoteLocalizationName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileProviderConfigurationItem"/> class.
        /// </summary>
        public FileProviderConfigurationItem()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileProviderConfigurationItem"/> class with specified parameters.
        /// </summary>
        /// <param name="valueType">The type of the configuration value.</param>
        /// <param name="eg">An example value for the configuration item.</param>
        /// <param name="noteLocalizationName">The localization name for notes or descriptions.</param>
        public FileProviderConfigurationItem(Type valueType, string eg, string noteLocalizationName)
        {
            ValueType = valueType;
            Eg = eg;
            NoteLocalizationName = noteLocalizationName;
        }
    }
}