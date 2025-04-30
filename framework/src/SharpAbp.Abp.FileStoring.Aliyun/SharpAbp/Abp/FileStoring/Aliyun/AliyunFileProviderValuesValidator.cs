using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring.Aliyun
{
    [ExposeKeyedService<IFileProviderValuesValidator>(AliyunFileProviderConfigurationNames.ProviderName)]
    public class AliyunFileProviderValuesValidator : BaseFileProviderValuesValidator, ITransientDependency
    {
        public override string Provider => AliyunFileProviderConfigurationNames.ProviderName;

        public AliyunFileProviderValuesValidator(IOptions<AbpFileStoringAbstractionsOptions> options) : base(options)
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


            //RegionId
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AliyunFileProviderConfigurationNames.RegionId, values.FindValue(AliyunFileProviderConfigurationNames.RegionId));

            //Endpoint
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AliyunFileProviderConfigurationNames.Endpoint, values.FindValue(AliyunFileProviderConfigurationNames.Endpoint));

            //BucketName
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AliyunFileProviderConfigurationNames.BucketName, values.FindValue(AliyunFileProviderConfigurationNames.BucketName));

            //AccessKeyId
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AliyunFileProviderConfigurationNames.AccessKeyId, values.FindValue(AliyunFileProviderConfigurationNames.AccessKeyId));

            //AccessKeySecret
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AliyunFileProviderConfigurationNames.AccessKeySecret, values.FindValue(AliyunFileProviderConfigurationNames.AccessKeySecret));

            //UseSecurityTokenService
            ValidateHelper.ShouldBool(result, Provider, AliyunFileProviderConfigurationNames.UseSecurityTokenService, values.FindValue(AliyunFileProviderConfigurationNames.UseSecurityTokenService));

            //DurationSeconds
            ValidateHelper.ShouldInt(result, Provider, AliyunFileProviderConfigurationNames.DurationSeconds, values.FindValue(AliyunFileProviderConfigurationNames.DurationSeconds));

            //CreateContainerIfNotExists
            ValidateHelper.ShouldBool(result, Provider, AliyunFileProviderConfigurationNames.CreateContainerIfNotExists, values.FindValue(AliyunFileProviderConfigurationNames.CreateContainerIfNotExists));

            //TemporaryCredentialsCacheKey
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AliyunFileProviderConfigurationNames.TemporaryCredentialsCacheKey, values.FindValue(AliyunFileProviderConfigurationNames.TemporaryCredentialsCacheKey));

            //RoleArn
            //ValidateHelper.NotNullOrWhiteSpace(result, Provider, AliyunFileProviderConfigurationNames.RoleArn, keyValuePairs[AliyunFileProviderConfigurationNames.RoleArn]);

            //Policy
            //ValidateHelper.NotNullOrWhiteSpace(result, Provider, AliyunFileProviderConfigurationNames.Policy, keyValuePairs[AliyunFileProviderConfigurationNames.Policy]);

            return result;
        }
    }
}
