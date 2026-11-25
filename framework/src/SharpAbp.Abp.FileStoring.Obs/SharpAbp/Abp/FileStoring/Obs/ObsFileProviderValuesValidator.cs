using Microsoft.Extensions.Options;
using OBS;
using SharpAbp.Abp.Core.Extensions;
using SharpAbp.Abp.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

            //AuthType
            var authType = values.FindValue(ObsFileProviderConfigurationNames.AuthType);
            if (!Enum.GetNames(typeof(AuthTypeEnum)).Any(x => string.Equals(x, authType, StringComparison.OrdinalIgnoreCase)))
            {
                result.Errors.Add(new ValidationResult($"[{Provider}-{ObsFileProviderConfigurationNames.AuthType}] value should be one of: {string.Join(", ", Enum.GetNames(typeof(AuthTypeEnum)))}."));
            }

            //AuthTypeNegotiation
            var authTypeNegotiation = values.FindValue(ObsFileProviderConfigurationNames.AuthTypeNegotiation);
            ValidateHelper.ShouldBool(result, Provider, ObsFileProviderConfigurationNames.AuthTypeNegotiation, authTypeNegotiation);
            if (bool.TryParse(authTypeNegotiation, out var negotiate) && negotiate &&
                !string.Equals(authType, nameof(AuthTypeEnum.OBS), StringComparison.OrdinalIgnoreCase))
            {
                result.Errors.Add(new ValidationResult($"[{Provider}-{ObsFileProviderConfigurationNames.AuthTypeNegotiation}] should be false when '{ObsFileProviderConfigurationNames.AuthType}' is not OBS, otherwise the SDK negotiates OBS/V2 and ignores the configured auth type."));
            }

            //CreateContainerIfNotExists
            ValidateHelper.ShouldBool(result, Provider, ObsFileProviderConfigurationNames.CreateContainerIfNotExists, values.FindValue(ObsFileProviderConfigurationNames.CreateContainerIfNotExists));

            return result;
        }
    }
}
