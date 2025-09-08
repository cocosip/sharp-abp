using Microsoft.Extensions.Options;
using SharpAbp.Abp.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
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

        public override IAbpValidationResult Validate(List<NameValue> values)
        {
            var result = new AbpValidationResult();
            ValidateBasic(result, values);
            if (result.Errors.Any())
            {
                return result;
            }

            //BucketName
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, KS3FileProviderConfigurationNames.BucketName, values.FindValue(KS3FileProviderConfigurationNames.BucketName));

            //Endpoint
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, KS3FileProviderConfigurationNames.Endpoint, values.FindValue(KS3FileProviderConfigurationNames.Endpoint));

            //AccessKeyId
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, KS3FileProviderConfigurationNames.AccessKey, values.FindValue(KS3FileProviderConfigurationNames.AccessKey));

            //SecretAccessKey
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, KS3FileProviderConfigurationNames.SecretKey, values.FindValue(KS3FileProviderConfigurationNames.SecretKey));

            //Protocol
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, KS3FileProviderConfigurationNames.Protocol, values.FindValue(KS3FileProviderConfigurationNames.Protocol));

            //UserAgent
            //ValidateHelper.NotNullOrWhiteSpace(result, Provider, KS3FileProviderConfigurationNames.UserAgent, values.FindValue[KS3FileProviderConfigurationNames.UserAgent]);

            //MaxConnections
            //ValidateHelper.ShouldInt(result, Provider, KS3FileProviderConfigurationNames.MaxConnections, values.FindValue[KS3FileProviderConfigurationNames.MaxConnections]);

            //Timeout
            //ValidateHelper.ShouldInt(result, Provider, KS3FileProviderConfigurationNames.Timeout, values.FindValue[KS3FileProviderConfigurationNames.Timeout]);

            //ReadWriteTimeout
            //ValidateHelper.ShouldInt(result, Provider, KS3FileProviderConfigurationNames.ReadWriteTimeout, values.FindValue[KS3FileProviderConfigurationNames.ReadWriteTimeout]);

            //CreateContainerIfNotExists
            ValidateHelper.ShouldBool(result, Provider, KS3FileProviderConfigurationNames.CreateContainerIfNotExists, values.FindValue(KS3FileProviderConfigurationNames.CreateContainerIfNotExists));

            return result;
        }
    }
}
