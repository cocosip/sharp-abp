using Microsoft.Extensions.Options;
using SharpAbp.Abp.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
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

        public override IAbpValidationResult Validate(List<NameValue> values)
        {
            var result = new AbpValidationResult();
            ValidateBasic(result, values);
            if (result.Errors.Any())
            {
                return result;
            }

            //BucketName
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AwsFileProviderConfigurationNames.ContainerName, values.FindValue(AwsFileProviderConfigurationNames.ContainerName));

            //AccessKeyId
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AwsFileProviderConfigurationNames.AccessKeyId, values.FindValue(AwsFileProviderConfigurationNames.AccessKeyId));

            //SecretAccessKey
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AwsFileProviderConfigurationNames.SecretAccessKey, values.FindValue(AwsFileProviderConfigurationNames.SecretAccessKey));

            //UseCredentials
            //ValidateHelper.ShouldBool(result, Provider, AwsFileProviderConfigurationNames.UseCredentials, values.FindValue[AwsFileProviderConfigurationNames.UseCredentials]);

            //UseTemporaryCredentials
            //ValidateHelper.ShouldBool(result, Provider, AwsFileProviderConfigurationNames.UseTemporaryCredentials, values.FindValue[AwsFileProviderConfigurationNames.UseTemporaryCredentials]);

            //UseTemporaryFederatedCredentials
            //ValidateHelper.ShouldBool(result, Provider, AwsFileProviderConfigurationNames.UseTemporaryFederatedCredentials, values.FindValue[AwsFileProviderConfigurationNames.UseTemporaryFederatedCredentials]);

            //CreateContainerIfNotExists
            ValidateHelper.ShouldBool(result, Provider, AwsFileProviderConfigurationNames.CreateContainerIfNotExists, values.FindValue(AwsFileProviderConfigurationNames.CreateContainerIfNotExists));


            //DurationSeconds
            //ValidateHelper.ShouldInt(result, Provider, AwsFileProviderConfigurationNames.DurationSeconds, values.FindValue[AwsFileProviderConfigurationNames.DurationSeconds]);

            ////ProfileName
            //ValidateHelper.NotNullOrWhiteSpace(result, Provider, AwsFileProviderConfigurationNames.ProfileName, values.FindValue[AwsFileProviderConfigurationNames.ProfileName]);

            ////ProfilesLocation
            //ValidateHelper.NotNullOrWhiteSpace(result, Provider, AwsFileProviderConfigurationNames.ProfilesLocation, values.FindValue[AwsFileProviderConfigurationNames.ProfilesLocation]);
 
            ////Name
            //ValidateHelper.NotNullOrWhiteSpace(result, Provider, AwsFileProviderConfigurationNames.Name, values.FindValue[AwsFileProviderConfigurationNames.Name]);

            ////Policy
            //ValidateHelper.NotNullOrWhiteSpace(result, Provider, AwsFileProviderConfigurationNames.Policy, values.FindValue[AwsFileProviderConfigurationNames.Policy]);

            ////Region
            //ValidateHelper.NotNullOrWhiteSpace(result, Provider, AwsFileProviderConfigurationNames.Region, values.FindValue[AwsFileProviderConfigurationNames.Region]);

            return result;
        }
    }
}
