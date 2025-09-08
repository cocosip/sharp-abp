using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.Core.Extensions;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring.S3
{
    [ExposeKeyedService<IFileProviderValuesValidator>(S3FileProviderConfigurationNames.ProviderName)]
    public class S3FileProviderValuesValidator : BaseFileProviderValuesValidator, ITransientDependency
    {
        public override string Provider => S3FileProviderConfigurationNames.ProviderName;
        public S3FileProviderValuesValidator(IOptions<AbpFileStoringAbstractionsOptions> options) : base(options)
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

            //BucketName
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, S3FileProviderConfigurationNames.BucketName, values.FindValue(S3FileProviderConfigurationNames.BucketName));

            //ServerUrl
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, S3FileProviderConfigurationNames.ServerUrl, values.FindValue(S3FileProviderConfigurationNames.ServerUrl));

            //AccessKeyId
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, S3FileProviderConfigurationNames.AccessKeyId, values.FindValue(S3FileProviderConfigurationNames.AccessKeyId));

            //SecretAccessKey
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, S3FileProviderConfigurationNames.SecretAccessKey, values.FindValue(S3FileProviderConfigurationNames.SecretAccessKey));

            //ForcePathStyle
            ValidateHelper.ShouldBool(result, Provider, S3FileProviderConfigurationNames.ForcePathStyle, values.FindValue(S3FileProviderConfigurationNames.ForcePathStyle));

            //UseChunkEncoding
            ValidateHelper.ShouldBool(result, Provider, S3FileProviderConfigurationNames.UseChunkEncoding, values.FindValue(S3FileProviderConfigurationNames.UseChunkEncoding));

            //Protocol
            ValidateHelper.ShouldInt(result, Provider, S3FileProviderConfigurationNames.Protocol, values.FindValue(S3FileProviderConfigurationNames.Protocol));

            //SignatureVersion
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, S3FileProviderConfigurationNames.SignatureVersion, values.FindValue(S3FileProviderConfigurationNames.SignatureVersion));

            //CreateBucketIfNotExists
            ValidateHelper.ShouldBool(result, Provider, S3FileProviderConfigurationNames.CreateBucketIfNotExists, values.FindValue(S3FileProviderConfigurationNames.CreateBucketIfNotExists));

            return result;
        }
    }
}
