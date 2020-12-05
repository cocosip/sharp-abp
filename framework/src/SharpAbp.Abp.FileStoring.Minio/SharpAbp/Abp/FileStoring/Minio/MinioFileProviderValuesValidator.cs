using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring.Minio
{
    public class MinioFileProviderValuesValidator : BaseFileProviderValuesValidator, ITransientDependency
    {
        public override string Provider => MinioFileProviderConfigurationNames.ProviderName;
        public MinioFileProviderValuesValidator(IOptions<AbpFileStoringOptions> options) : base(options)
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

            //BucketName
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, MinioFileProviderConfigurationNames.BucketName, keyValuePairs[MinioFileProviderConfigurationNames.BucketName]);

            //EndPoint
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, MinioFileProviderConfigurationNames.EndPoint, keyValuePairs[MinioFileProviderConfigurationNames.EndPoint]);

            //AccessKey
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, MinioFileProviderConfigurationNames.AccessKey, keyValuePairs[MinioFileProviderConfigurationNames.AccessKey]);

            //SecretKey
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, MinioFileProviderConfigurationNames.SecretKey, keyValuePairs[MinioFileProviderConfigurationNames.SecretKey]);

            //WithSSL
            ValidateHelper.ShouldBool(result, Provider, MinioFileProviderConfigurationNames.WithSSL, keyValuePairs[MinioFileProviderConfigurationNames.WithSSL]);

            //CreateBucketIfNotExists
            ValidateHelper.ShouldBool(result, Provider, MinioFileProviderConfigurationNames.CreateBucketIfNotExists, keyValuePairs[MinioFileProviderConfigurationNames.CreateBucketIfNotExists]);

            return result;
        }
    }
}
