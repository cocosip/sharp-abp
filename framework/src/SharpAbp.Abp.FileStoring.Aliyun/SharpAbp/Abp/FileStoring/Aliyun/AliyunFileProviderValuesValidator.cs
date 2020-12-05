using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring.Aliyun
{
    public class AliyunFileProviderValuesValidator : BaseFileProviderValuesValidator, ITransientDependency
    {
        public override string Provider => AliyunFileProviderConfigurationNames.ProviderName;

        public AliyunFileProviderValuesValidator(IOptions<AbpFileStoringOptions> options) : base(options)
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

            //RegionId
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AliyunFileProviderConfigurationNames.RegionId, keyValuePairs[AliyunFileProviderConfigurationNames.RegionId]);

            //Endpoint
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AliyunFileProviderConfigurationNames.Endpoint, keyValuePairs[AliyunFileProviderConfigurationNames.Endpoint]);

            //BucketName
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AliyunFileProviderConfigurationNames.BucketName, keyValuePairs[AliyunFileProviderConfigurationNames.BucketName]);

            //AccessKeyId
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AliyunFileProviderConfigurationNames.AccessKeyId, keyValuePairs[AliyunFileProviderConfigurationNames.AccessKeyId]);

            //AccessKeySecret
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AliyunFileProviderConfigurationNames.AccessKeySecret, keyValuePairs[AliyunFileProviderConfigurationNames.AccessKeySecret]);

            //UseSecurityTokenService
            ValidateHelper.ShouldBool(result, Provider, AliyunFileProviderConfigurationNames.UseSecurityTokenService, keyValuePairs[AliyunFileProviderConfigurationNames.UseSecurityTokenService]);

            //DurationSeconds
            ValidateHelper.ShouldInt(result, Provider, AliyunFileProviderConfigurationNames.DurationSeconds, keyValuePairs[AliyunFileProviderConfigurationNames.DurationSeconds]);

            //CreateContainerIfNotExists
            ValidateHelper.ShouldBool(result, Provider, AliyunFileProviderConfigurationNames.CreateContainerIfNotExists, keyValuePairs[AliyunFileProviderConfigurationNames.CreateContainerIfNotExists]);

            //TemporaryCredentialsCacheKey
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, AliyunFileProviderConfigurationNames.TemporaryCredentialsCacheKey, keyValuePairs[AliyunFileProviderConfigurationNames.TemporaryCredentialsCacheKey]);

            //RoleArn
            //ValidateHelper.NotNullOrWhiteSpace(result, Provider, AliyunFileProviderConfigurationNames.RoleArn, keyValuePairs[AliyunFileProviderConfigurationNames.RoleArn]);

            //Policy
            //ValidateHelper.NotNullOrWhiteSpace(result, Provider, AliyunFileProviderConfigurationNames.Policy, keyValuePairs[AliyunFileProviderConfigurationNames.Policy]);

            return result;
        }
    }
}
