using Microsoft.Extensions.Options;
using SharpAbp.Abp.Core.Extensions;
using SharpAbp.Abp.Validation;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring.FastDFS
{
    [ExposeKeyedService<IFileProviderValuesValidator>(FastDFSFileProviderConfigurationNames.ProviderName)]
    public class FastDFSFileProviderValuesValidator : BaseFileProviderValuesValidator, ITransientDependency
    {
        public override string Provider => FastDFSFileProviderConfigurationNames.ProviderName;
        public FastDFSFileProviderValuesValidator(IOptions<AbpFileStoringAbstractionsOptions> options) : base(options)
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

            //ClusterName
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, FastDFSFileProviderConfigurationNames.ClusterName, values.FindValue(FastDFSFileProviderConfigurationNames.ClusterName));

            //HttpServer
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, FastDFSFileProviderConfigurationNames.HttpServer, values.FindValue(FastDFSFileProviderConfigurationNames.HttpServer));

            //GroupName
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, FastDFSFileProviderConfigurationNames.GroupName, values.FindValue(FastDFSFileProviderConfigurationNames.GroupName));

            //AppendGroupNameToUrl
            ValidateHelper.ShouldBool(result, Provider, FastDFSFileProviderConfigurationNames.AppendGroupNameToUrl, values.FindValue(FastDFSFileProviderConfigurationNames.AppendGroupNameToUrl));

            //Trackers
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, FastDFSFileProviderConfigurationNames.Trackers, values.FindValue(FastDFSFileProviderConfigurationNames.Trackers));

            //AntiStealCheckToken
            ValidateHelper.ShouldBool(result, Provider, FastDFSFileProviderConfigurationNames.AntiStealCheckToken, values.FindValue(FastDFSFileProviderConfigurationNames.AntiStealCheckToken));

            ////SecretKey
            //ValidateHelper.NotNullOrWhiteSpace(result, Provider, FastDFSFileProviderConfigurationNames.SecretKey, values.FindValue[FastDFSFileProviderConfigurationNames.SecretKey]);

            //Charset
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, FastDFSFileProviderConfigurationNames.Charset, values.FindValue(FastDFSFileProviderConfigurationNames.Charset));

            //ConnectionTimeout
            ValidateHelper.ShouldInt(result, Provider, FastDFSFileProviderConfigurationNames.ConnectionTimeout, values.FindValue(FastDFSFileProviderConfigurationNames.ConnectionTimeout));

            //ConnectionLifeTime
            ValidateHelper.ShouldInt(result, Provider, FastDFSFileProviderConfigurationNames.ConnectionLifeTime, values.FindValue(FastDFSFileProviderConfigurationNames.ConnectionLifeTime));

            //ConnectionConcurrentThread
            ValidateHelper.ShouldInt(result, Provider, FastDFSFileProviderConfigurationNames.ConnectionConcurrentThread, values.FindValue(FastDFSFileProviderConfigurationNames.ConnectionConcurrentThread));

            //ScanTimeoutConnectionInterval
            ValidateHelper.ShouldInt(result, Provider, FastDFSFileProviderConfigurationNames.ScanTimeoutConnectionInterval, values.FindValue(FastDFSFileProviderConfigurationNames.ScanTimeoutConnectionInterval));

            //TrackerMaxConnection
            ValidateHelper.ShouldInt(result, Provider, FastDFSFileProviderConfigurationNames.TrackerMaxConnection, values.FindValue(FastDFSFileProviderConfigurationNames.TrackerMaxConnection));

            //StorageMaxConnection
            ValidateHelper.ShouldInt(result, Provider, FastDFSFileProviderConfigurationNames.StorageMaxConnection, values.FindValue(FastDFSFileProviderConfigurationNames.StorageMaxConnection));

            return result;
        }


    }
}
