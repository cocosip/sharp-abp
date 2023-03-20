using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring.Obs
{
    public class ObsFileProviderValuesValidator : BaseFileProviderValuesValidator, ITransientDependency
    {
        public override string Provider => ObsFileProviderConfigurationNames.ProviderName;

        public ObsFileProviderValuesValidator(IOptions<AbpFileStoringAbstractionsOptions> options) : base(options)
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
            // ValidateHelper.NotNullOrWhiteSpace(result, Provider, ObsFileProviderConfigurationNames.RegionId, keyValuePairs[ObsFileProviderConfigurationNames.RegionId]);

            //Endpoint
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, ObsFileProviderConfigurationNames.Endpoint, keyValuePairs[ObsFileProviderConfigurationNames.Endpoint]);

            //BucketName
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, ObsFileProviderConfigurationNames.BucketName, keyValuePairs[ObsFileProviderConfigurationNames.BucketName]);

            //AccessKeyId
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, ObsFileProviderConfigurationNames.AccessKeyId, keyValuePairs[ObsFileProviderConfigurationNames.AccessKeyId]);

            //AccessKeySecret
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, ObsFileProviderConfigurationNames.AccessKeySecret, keyValuePairs[ObsFileProviderConfigurationNames.AccessKeySecret]);

            //CreateContainerIfNotExists
            ValidateHelper.ShouldBool(result, Provider, ObsFileProviderConfigurationNames.CreateContainerIfNotExists, keyValuePairs[ObsFileProviderConfigurationNames.CreateContainerIfNotExists]);

            return result;
        }
    }
}
