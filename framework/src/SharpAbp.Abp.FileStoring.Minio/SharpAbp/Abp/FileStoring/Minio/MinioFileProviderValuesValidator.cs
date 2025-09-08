using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.Core.Extensions;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring.Minio
{
    [ExposeKeyedService<IFileProviderValuesValidator>(MinioFileProviderConfigurationNames.ProviderName)]
    public class MinioFileProviderValuesValidator : BaseFileProviderValuesValidator, ITransientDependency
    {
        public override string Provider => MinioFileProviderConfigurationNames.ProviderName;
        public MinioFileProviderValuesValidator(IOptions<AbpFileStoringAbstractionsOptions> options) : base(options)
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
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, MinioFileProviderConfigurationNames.BucketName, values.FindValue(MinioFileProviderConfigurationNames.BucketName));

            //EndPoint
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, MinioFileProviderConfigurationNames.EndPoint, values.FindValue(MinioFileProviderConfigurationNames.EndPoint));

            //AccessKey
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, MinioFileProviderConfigurationNames.AccessKey, values.FindValue(MinioFileProviderConfigurationNames.AccessKey));

            //SecretKey
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, MinioFileProviderConfigurationNames.SecretKey, values.FindValue(MinioFileProviderConfigurationNames.SecretKey));

            //WithSSL
            ValidateHelper.ShouldBool(result, Provider, MinioFileProviderConfigurationNames.WithSSL, values.FindValue(MinioFileProviderConfigurationNames.WithSSL));

            //CreateBucketIfNotExists
            ValidateHelper.ShouldBool(result, Provider, MinioFileProviderConfigurationNames.CreateBucketIfNotExists, values.FindValue(MinioFileProviderConfigurationNames.CreateBucketIfNotExists));

            return result;
        }
    }
}
