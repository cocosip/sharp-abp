using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    /// <summary>
    /// Internal extension methods for property dictionary operations.
    /// </summary>
    internal static class PropertyExtensions
    {
        /// <summary>
        /// Gets a property value from a dictionary with validation and default value.
        /// </summary>
        /// <param name="properties">The properties dictionary.</param>
        /// <param name="propertyName">The name of the property to retrieve.</param>
        /// <param name="defaultValue">The default value if property is not found.</param>
        /// <param name="validValues">Optional set of valid values for validation.</param>
        /// <returns>The property value if found and valid; otherwise, the default value.</returns>
        public static string GetPropertyValue(
            this Dictionary<string, string>? properties,
            string propertyName,
            string defaultValue,
            HashSet<string>? validValues = null)
        {
            Check.NotNullOrWhiteSpace(propertyName, nameof(propertyName));
            Check.NotNull(defaultValue, nameof(defaultValue));

            if (properties?.TryGetValue(propertyName, out string? value) == true && !value.IsNullOrWhiteSpace())
            {
                // If valid values are specified, check if the value is in the valid set
                if (validValues != null)
                {
                    return validValues.Contains(value) ? value : defaultValue;
                }

                // If no validation is needed, return the value
                return value;
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets a property value from SharpAbpEfCoreOptions with validation and default value.
        /// </summary>
        /// <param name="options">The SharpAbpEfCoreOptions instance.</param>
        /// <param name="propertyName">The name of the property to retrieve.</param>
        /// <param name="defaultValue">The default value to return if the property is not found.</param>
        /// <param name="validValues">Optional set of valid values for validation.</param>
        /// <returns>The property value if found and valid; otherwise, the default value.</returns>
        public static string GetPropertyValue(
            this SharpAbpEfCoreOptions options,
            string propertyName,
            string defaultValue,
            HashSet<string>? validValues = null)
        {
            Check.NotNull(options, nameof(options));
            Check.NotNullOrWhiteSpace(propertyName, nameof(propertyName));
            Check.NotNull(defaultValue, nameof(defaultValue));
            
            if (!options.Properties.TryGetValue(propertyName, out string? value) || value.IsNullOrWhiteSpace())
            {
                return defaultValue;
            }

            // If valid values are specified, check if the value is in the valid set
            if (validValues != null)
            {
                return validValues.Contains(value) ? value : defaultValue;
            }

            return value;
        }
    }
}