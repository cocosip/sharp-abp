using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Base class for file provider values validators.
    /// Provides common validation functionality for file storage provider configurations.
    /// </summary>
    public abstract class BaseFileProviderValuesValidator : IFileProviderValuesValidator
    {
        /// <summary>
        /// Configuration options for file storing abstractions.
        /// </summary>
        protected AbpFileStoringAbstractionsOptions Options { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">Configuration options for file storing abstractions</param>
        public BaseFileProviderValuesValidator(IOptions<AbpFileStoringAbstractionsOptions> options)
        {
            Options = options.Value;
        }

        /// <summary>
        /// Gets the provider name that this validator is for.
        /// </summary>
        public abstract string Provider { get; }
        
        /// <summary>
        /// Validates the provided configuration values for the file provider.
        /// </summary>
        /// <param name="values">List of configuration values to validate</param>
        /// <returns>Validation result containing any validation errors</returns>
        public abstract IAbpValidationResult Validate(List<NameValue> values);

        /// <summary>
        /// Performs basic validation of required configuration items.
        /// Checks that all required configuration items for the provider are present.
        /// </summary>
        /// <param name="result">Validation result to add errors to</param>
        /// <param name="values">List of configuration values to validate</param>
        protected virtual void ValidateBasic(IAbpValidationResult result, List<NameValue> values)
        {
            var providerConfiguration = Options.Providers.GetConfiguration(Provider) ?? 
                throw new AbpException($"File provider configuration not found for provider '{Provider}'. Please check your configuration.");

            foreach (var itemKeyValuePair in providerConfiguration.GetItems())
            {
                var value = values.FirstOrDefault(x => x.Name == itemKeyValuePair.Key);
                if (value == null)
                {
                    result.Errors.Add(new ValidationResult($"Required configuration item '[{Provider}-{itemKeyValuePair.Key}]' is missing. Please provide a value for this required item."));
                }
            }
        }
    }
}