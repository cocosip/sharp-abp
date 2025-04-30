using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.Options;
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
        public abstract IAbpValidationResult Validate(List<NameValue> values);

        protected virtual void ValidateBasic(IAbpValidationResult result, List<NameValue> values)
        {
            var providerConfiguration = Options.Providers.GetConfiguration(Provider) ?? throw new AbpException($"Could not find any provider configuration for provider '{Provider}'.");

            foreach (var itemKeyValuePair in providerConfiguration.GetItems())
            {
                var value = values.FirstOrDefault(x => x.Name == itemKeyValuePair.Key);
                if (value == null)
                {
                    result.Errors.Add(new ValidationResult($"[{Provider}-{itemKeyValuePair.Key}] is missing."));
                }
            }
        }
    }
}
