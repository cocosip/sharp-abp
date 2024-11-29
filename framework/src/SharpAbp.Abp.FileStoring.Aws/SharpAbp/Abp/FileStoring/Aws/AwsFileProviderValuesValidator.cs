using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring.Aws
{
    [ExposeKeyedService<IFileProviderValuesValidator>(AwsFileProviderConfigurationNames.ProviderName)]
    public class AwsFileProviderValuesValidator : BaseFileProviderValuesValidator, ITransientDependency
    {
        public AwsFileProviderValuesValidator(IOptions<AbpFileStoringAbstractionsOptions> options) : base(options)
        {
        }

        public override string Provider => AwsFileProviderConfigurationNames.ProviderName;

        public override IAbpValidationResult Validate(Dictionary<string, string> keyValuePairs)
        {
            var result = new AbpValidationResult();
            ValidateBasic(result, keyValuePairs);
            if (result.Errors.Any())
            {
                return result;
            }

            //BucketName
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AwsFileProviderConfigurationNames.ContainerName, keyValuePairs[AwsFileProviderConfigurationNames.ContainerName]);


            //AccessKeyId
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AwsFileProviderConfigurationNames.AccessKeyId, keyValuePairs[AwsFileProviderConfigurationNames.AccessKeyId]);

            //SecretAccessKey
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AwsFileProviderConfigurationNames.SecretAccessKey, keyValuePairs[AwsFileProviderConfigurationNames.SecretAccessKey]);

            //UseCredentials
            //ValidateHelper.ShouldBool(result, Provider, AwsFileProviderConfigurationNames.UseCredentials, keyValuePairs[AwsFileProviderConfigurationNames.UseCredentials]);

            //UseTemporaryCredentials
            //ValidateHelper.ShouldBool(result, Provider, AwsFileProviderConfigurationNames.UseTemporaryCredentials, keyValuePairs[AwsFileProviderConfigurationNames.UseTemporaryCredentials]);

            //UseTemporaryFederatedCredentials
            //ValidateHelper.ShouldBool(result, Provider, AwsFileProviderConfigurationNames.UseTemporaryFederatedCredentials, keyValuePairs[AwsFileProviderConfigurationNames.UseTemporaryFederatedCredentials]);

            //CreateContainerIfNotExists
            ValidateHelper.ShouldBool(result, Provider, AwsFileProviderConfigurationNames.CreateContainerIfNotExists, keyValuePairs[AwsFileProviderConfigurationNames.CreateContainerIfNotExists]);


            //DurationSeconds
            //ValidateHelper.ShouldInt(result, Provider, AwsFileProviderConfigurationNames.DurationSeconds, keyValuePairs[AwsFileProviderConfigurationNames.DurationSeconds]);

            ////ProfileName
            //ValidateHelper.NotNullOrWhiteSpace(result, Provider, AwsFileProviderConfigurationNames.ProfileName, keyValuePairs[AwsFileProviderConfigurationNames.ProfileName]);

            ////ProfilesLocation
            //ValidateHelper.NotNullOrWhiteSpace(result, Provider, AwsFileProviderConfigurationNames.ProfilesLocation, keyValuePairs[AwsFileProviderConfigurationNames.ProfilesLocation]);
 
            ////Name
            //ValidateHelper.NotNullOrWhiteSpace(result, Provider, AwsFileProviderConfigurationNames.Name, keyValuePairs[AwsFileProviderConfigurationNames.Name]);

            ////Policy
            //ValidateHelper.NotNullOrWhiteSpace(result, Provider, AwsFileProviderConfigurationNames.Policy, keyValuePairs[AwsFileProviderConfigurationNames.Policy]);

            ////Region
            //ValidateHelper.NotNullOrWhiteSpace(result, Provider, AwsFileProviderConfigurationNames.Region, keyValuePairs[AwsFileProviderConfigurationNames.Region]);

            return result;
        }
    }
}
