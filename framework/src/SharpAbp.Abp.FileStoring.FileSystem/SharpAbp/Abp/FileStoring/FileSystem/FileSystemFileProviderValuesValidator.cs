using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.Core.Extensions;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring.FileSystem
{
    [ExposeKeyedService<IFileProviderValuesValidator>(FileSystemFileProviderConfigurationNames.ProviderName)]
    public class FileSystemFileProviderValuesValidator : BaseFileProviderValuesValidator, ITransientDependency
    {
        public override string Provider => FileSystemFileProviderConfigurationNames.ProviderName;

        public FileSystemFileProviderValuesValidator(IOptions<AbpFileStoringAbstractionsOptions> options) : base(options)
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

            //BasePath
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, FileSystemFileProviderConfigurationNames.BasePath, values.FindValue(FileSystemFileProviderConfigurationNames.BasePath));

            //AppendContainerNameToBasePath
            ValidateHelper.ShouldBool(result, Provider, FileSystemFileProviderConfigurationNames.AppendContainerNameToBasePath, values.FindValue(FileSystemFileProviderConfigurationNames.AppendContainerNameToBasePath));

            //HttpServer
            //ValidateHelper.NotNullOrWhiteSpace(result, Provider, FileSystemFileProviderConfigurationNames.HttpServer, values.FindValue[FileSystemFileProviderConfigurationNames.HttpServer]);

            return result;
        }
    }
}
