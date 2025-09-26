using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Defines the interface for validating file provider configuration values.
    /// </summary>
    public interface IFileProviderValuesValidator
    {
        /// <summary>
        /// Gets the name of the file provider this validator is for.
        /// </summary>
        string Provider { get; }

        /// <summary>
        /// Validates the provided configuration values for the file provider.
        /// </summary>
        /// <param name="values">The list of configuration values to validate.</param>
        /// <returns>The validation result containing any validation errors.</returns>
        IAbpValidationResult Validate(List<NameValue> values);
    }
}