using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring
{
    public abstract class BaseFileProviderValuesValidator : IFileProviderValuesValidator
    {
        protected AbpFileStoringAbstractionsOptions Options { get; }

        public BaseFileProviderValuesValidator(IOptions<AbpFileStoringAbstractionsOptions> options)
        {
            Options = options.Value;
        }

        public abstract string Provider { get; }
        public abstract IAbpValidationResult Validate(Dictionary<string, string> keyValuePairs);

        protected virtual void ValidateBasic(IAbpValidationResult result, Dictionary<string, string> keyValuePairs)
        {
            var providerConfiguration = Options.Providers.GetConfiguration(Provider) ?? throw new AbpException($"Could not find any provider configuration for provider '{Provider}'.");

            foreach (var itemKeyValuePair in providerConfiguration.GetItems())
            {
                if (!keyValuePairs.ContainsKey(itemKeyValuePair.Key))
                {
                    result.Errors.Add(new ValidationResult($"[{Provider}-{itemKeyValuePair.Key}] is missing."));
                }
            }
        }
    }
}
