using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring
{
    public abstract class BaseFileProviderValuesValidator : IFileProviderValuesValidator
    {
        protected AbpFileStoringOptions Options { get; }

        public BaseFileProviderValuesValidator(IOptions<AbpFileStoringOptions> options)
        {
            Options = options.Value;
        }

        public abstract string Provider { get; }
        public abstract IAbpValidationResult Validate(Dictionary<string, string> keyValuePairs);

        protected virtual void ValidateBasic(IAbpValidationResult result, Dictionary<string, string> keyValuePairs)
        {
            var providerConfiguration = Options.Providers.GetConfiguration(Provider);
            if (providerConfiguration == null)
            {
                throw new AbpException($"Could not find any provider configuration for provider '{Provider}'.");
            }

            foreach (var providerValueKv in providerConfiguration.GetValues())
            {
                if (!keyValuePairs.ContainsKey(providerValueKv.Key))
                {
                    result.Errors.Add(new ValidationResult($"[{Provider}-{providerValueKv.Key}] is missing."));
                }
            }
        }
    }
}
