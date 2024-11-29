using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring.KS3
{
    [ExposeKeyedService<IFileProviderValuesValidator>(KS3FileProviderConfigurationNames.ProviderName)]
    public class KS3FileProviderValuesValidator : BaseFileProviderValuesValidator, ITransientDependency
    {
        public KS3FileProviderValuesValidator(IOptions<AbpFileStoringAbstractionsOptions> options) : base(options)
        {
        }

        public override string Provider => KS3FileProviderConfigurationNames.ProviderName;

        public override IAbpValidationResult Validate(Dictionary<string, string> keyValuePairs)
        {
            var result = new AbpValidationResult();
            ValidateBasic(result, keyValuePairs);
            if (result.Errors.Any())
            {
                return result;
            }

            //BucketName
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, KS3FileProviderConfigurationNames.BucketName, keyValuePairs[KS3FileProviderConfigurationNames.BucketName]);

            //Endpoint
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, KS3FileProviderConfigurationNames.Endpoint, keyValuePairs[KS3FileProviderConfigurationNames.Endpoint]);

            //AccessKeyId
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, KS3FileProviderConfigurationNames.AccessKey, keyValuePairs[KS3FileProviderConfigurationNames.AccessKey]);

            //SecretAccessKey
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, KS3FileProviderConfigurationNames.SecretKey, keyValuePairs[KS3FileProviderConfigurationNames.SecretKey]);

            //Protocol
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, KS3FileProviderConfigurationNames.Protocol, keyValuePairs[KS3FileProviderConfigurationNames.Protocol]);

            //UserAgent
            //ValidateHelper.NotNullOrWhiteSpace(result, Provider, KS3FileProviderConfigurationNames.UserAgent, keyValuePairs[KS3FileProviderConfigurationNames.UserAgent]);

            //MaxConnections
            //ValidateHelper.ShouldInt(result, Provider, KS3FileProviderConfigurationNames.MaxConnections, keyValuePairs[KS3FileProviderConfigurationNames.MaxConnections]);

            //Timeout
            //ValidateHelper.ShouldInt(result, Provider, KS3FileProviderConfigurationNames.Timeout, keyValuePairs[KS3FileProviderConfigurationNames.Timeout]);

            //ReadWriteTimeout
            //ValidateHelper.ShouldInt(result, Provider, KS3FileProviderConfigurationNames.ReadWriteTimeout, keyValuePairs[KS3FileProviderConfigurationNames.ReadWriteTimeout]);

            //CreateContainerIfNotExists
            ValidateHelper.ShouldBool(result, Provider, KS3FileProviderConfigurationNames.CreateContainerIfNotExists, keyValuePairs[KS3FileProviderConfigurationNames.CreateContainerIfNotExists]);

            return result;
        }
    }
}
