using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Volo.Abp;
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

        public override IAbpValidationResult Validate(List<NameValue> values)
        {
            var result = new AbpValidationResult();
            ValidateBasic(result, values);

            if (result.Errors.Any())
            {
                return result;
            }

            //ConnectionString
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AzureFileProviderConfigurationNames.ConnectionString, values.FindValue(AzureFileProviderConfigurationNames.ConnectionString));

            //ContainerName
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AzureFileProviderConfigurationNames.ContainerName, values.FindValue(AzureFileProviderConfigurationNames.ContainerName));

            //CreateContainerIfNotExists
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AzureFileProviderConfigurationNames.CreateContainerIfNotExists, values.FindValue(AzureFileProviderConfigurationNames.CreateContainerIfNotExists));

            return result;
        }
    }
}
