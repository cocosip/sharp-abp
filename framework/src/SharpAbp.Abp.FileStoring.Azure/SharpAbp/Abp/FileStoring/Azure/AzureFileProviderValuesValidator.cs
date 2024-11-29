using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring.Azure
{
    [ExposeKeyedService<IFileProviderValuesValidator>(AzureFileProviderConfigurationNames.ProviderName)]
    public class AzureFileProviderValuesValidator : BaseFileProviderValuesValidator, ITransientDependency
    {
        public override string Provider => AzureFileProviderConfigurationNames.ProviderName;

        public AzureFileProviderValuesValidator(IOptions<AbpFileStoringAbstractionsOptions> options) : base(options)
        {

        }

        public override IAbpValidationResult Validate(Dictionary<string, string> keyValuePairs)
        {
            var result = new AbpValidationResult();
            ValidateBasic(result, keyValuePairs);

            if (result.Errors.Any())
            {
                return result;
            }

            //ConnectionString
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AzureFileProviderConfigurationNames.ConnectionString, keyValuePairs[AzureFileProviderConfigurationNames.ConnectionString]);

            //ContainerName
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AzureFileProviderConfigurationNames.ContainerName, keyValuePairs[AzureFileProviderConfigurationNames.ContainerName]);

            //CreateContainerIfNotExists
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AzureFileProviderConfigurationNames.CreateContainerIfNotExists, keyValuePairs[AzureFileProviderConfigurationNames.CreateContainerIfNotExists]);

            return result;
        }
    }
}
