using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring.S3
{
    public class S3FileProviderValuesValidator : BaseFileProviderValuesValidator, ITransientDependency
    {
        public override string Provider => S3FileProviderConfigurationNames.ProviderName;
        public S3FileProviderValuesValidator(IOptions<AbpFileStoringOptions> options) : base(options)
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
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, S3FileProviderConfigurationNames.BucketName, keyValuePairs[S3FileProviderConfigurationNames.BucketName]);

            //ServerUrl
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, S3FileProviderConfigurationNames.ServerUrl, keyValuePairs[S3FileProviderConfigurationNames.ServerUrl]);

            //AccessKeyId
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, S3FileProviderConfigurationNames.AccessKeyId, keyValuePairs[S3FileProviderConfigurationNames.AccessKeyId]);

            //SecretAccessKey
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, S3FileProviderConfigurationNames.SecretAccessKey, keyValuePairs[S3FileProviderConfigurationNames.SecretAccessKey]);

            //ForcePathStyle
            ValidateHelper.ShouldBool(result, Provider, S3FileProviderConfigurationNames.ForcePathStyle, keyValuePairs[S3FileProviderConfigurationNames.ForcePathStyle]);

            //UseChunkEncoding
            ValidateHelper.ShouldBool(result, Provider, S3FileProviderConfigurationNames.UseChunkEncoding, keyValuePairs[S3FileProviderConfigurationNames.UseChunkEncoding]);

            //Protocol
            ValidateHelper.ShouldInt(result, Provider, S3FileProviderConfigurationNames.Protocol, keyValuePairs[S3FileProviderConfigurationNames.Protocol]);

            //EnableSlice
            ValidateHelper.ShouldBool(result, Provider, S3FileProviderConfigurationNames.EnableSlice, keyValuePairs[S3FileProviderConfigurationNames.EnableSlice]);

            //SliceSize
            ValidateHelper.ShouldInt(result, Provider, S3FileProviderConfigurationNames.SliceSize, keyValuePairs[S3FileProviderConfigurationNames.SliceSize]);

            //SignatureVersion
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, S3FileProviderConfigurationNames.SignatureVersion, keyValuePairs[S3FileProviderConfigurationNames.SignatureVersion]);

            //CreateBucketIfNotExists
            ValidateHelper.ShouldBool(result, Provider, S3FileProviderConfigurationNames.CreateBucketIfNotExists, keyValuePairs[S3FileProviderConfigurationNames.CreateBucketIfNotExists]);

            return result;
        }
    }
}
