using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring.Obs
{
    [ExposeKeyedService<IFileProviderValuesValidator>(ObsFileProviderConfigurationNames.ProviderName)]
    public class ObsFileProviderValuesValidator : BaseFileProviderValuesValidator, ITransientDependency
    {
        public override string Provider => ObsFileProviderConfigurationNames.ProviderName;

        public ObsFileProviderValuesValidator(IOptions<AbpFileStoringAbstractionsOptions> options) : base(options)
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
            // ValidateHelper.NotNullOrWhiteSpace(result, Provider, ObsFileProviderConfigurationNames.RegionId, values.FindValue[ObsFileProviderConfigurationNames.RegionId]);

            //Endpoint
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, ObsFileProviderConfigurationNames.Endpoint, values.FindValue(ObsFileProviderConfigurationNames.Endpoint));

            //BucketName
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, ObsFileProviderConfigurationNames.BucketName, values.FindValue(ObsFileProviderConfigurationNames.BucketName));

            //AccessKeyId
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, ObsFileProviderConfigurationNames.AccessKeyId, values.FindValue(ObsFileProviderConfigurationNames.AccessKeyId));

            //AccessKeySecret
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, ObsFileProviderConfigurationNames.AccessKeySecret, values.FindValue(ObsFileProviderConfigurationNames.AccessKeySecret));

            //CreateContainerIfNotExists
            ValidateHelper.ShouldBool(result, Provider, ObsFileProviderConfigurationNames.CreateContainerIfNotExists, values.FindValue(ObsFileProviderConfigurationNames.CreateContainerIfNotExists));

            return result;
        }
    }
}
