using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring.FileSystem
{
    public class FileSystemFileProviderValuesValidator : BaseFileProviderValuesValidator, ITransientDependency
    {
        public override string Provider => FileSystemFileProviderConfigurationNames.ProviderName;

        public FileSystemFileProviderValuesValidator(IOptions<AbpFileStoringOptions> options) : base(options)
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

            //BasePath
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, FileSystemFileProviderConfigurationNames.BasePath, keyValuePairs[FileSystemFileProviderConfigurationNames.BasePath]);

            //AppendContainerNameToBasePath
            ValidateHelper.ShouldBool(result, Provider, FileSystemFileProviderConfigurationNames.AppendContainerNameToBasePath, keyValuePairs[FileSystemFileProviderConfigurationNames.AppendContainerNameToBasePath]);

            //HttpServer
            //ValidateHelper.NotNullOrWhiteSpace(result, Provider, FileSystemFileProviderConfigurationNames.HttpServer, keyValuePairs[FileSystemFileProviderConfigurationNames.HttpServer]);

            return result;
        }
    }
}
