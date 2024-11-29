using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
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


        public override IAbpValidationResult Validate(Dictionary<string, string> keyValuePairs)
        {
            var result = new AbpValidationResult();
            ValidateBasic(result, keyValuePairs);

            if (result.Errors.Any())
            {
                return result;
            }

            //ClusterName
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, FastDFSFileProviderConfigurationNames.ClusterName, keyValuePairs[FastDFSFileProviderConfigurationNames.ClusterName]);

            //HttpServer
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, FastDFSFileProviderConfigurationNames.HttpServer, keyValuePairs[FastDFSFileProviderConfigurationNames.HttpServer]);

            //GroupName
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, FastDFSFileProviderConfigurationNames.GroupName, keyValuePairs[FastDFSFileProviderConfigurationNames.GroupName]);

            //AppendGroupNameToUrl
            ValidateHelper.ShouldBool(result, Provider, FastDFSFileProviderConfigurationNames.AppendGroupNameToUrl, keyValuePairs[FastDFSFileProviderConfigurationNames.AppendGroupNameToUrl]);

            //Trackers
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, FastDFSFileProviderConfigurationNames.Trackers, keyValuePairs[FastDFSFileProviderConfigurationNames.Trackers]);

            //AntiStealCheckToken
            ValidateHelper.ShouldBool(result, Provider, FastDFSFileProviderConfigurationNames.AntiStealCheckToken, keyValuePairs[FastDFSFileProviderConfigurationNames.AntiStealCheckToken]);

            ////SecretKey
            //ValidateHelper.NotNullOrWhiteSpace(result, Provider, FastDFSFileProviderConfigurationNames.SecretKey, keyValuePairs[FastDFSFileProviderConfigurationNames.SecretKey]);

            //Charset
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, FastDFSFileProviderConfigurationNames.Charset, keyValuePairs[FastDFSFileProviderConfigurationNames.Charset]);

            //ConnectionTimeout
            ValidateHelper.ShouldInt(result, Provider, FastDFSFileProviderConfigurationNames.ConnectionTimeout, keyValuePairs[FastDFSFileProviderConfigurationNames.ConnectionTimeout]);

            //ConnectionLifeTime
            ValidateHelper.ShouldInt(result, Provider, FastDFSFileProviderConfigurationNames.ConnectionLifeTime, keyValuePairs[FastDFSFileProviderConfigurationNames.ConnectionLifeTime]);

            //ConnectionConcurrentThread
            ValidateHelper.ShouldInt(result, Provider, FastDFSFileProviderConfigurationNames.ConnectionConcurrentThread, keyValuePairs[FastDFSFileProviderConfigurationNames.ConnectionConcurrentThread]);

            //ScanTimeoutConnectionInterval
            ValidateHelper.ShouldInt(result, Provider, FastDFSFileProviderConfigurationNames.ScanTimeoutConnectionInterval, keyValuePairs[FastDFSFileProviderConfigurationNames.ScanTimeoutConnectionInterval]);

            //TrackerMaxConnection
            ValidateHelper.ShouldInt(result, Provider, FastDFSFileProviderConfigurationNames.TrackerMaxConnection, keyValuePairs[FastDFSFileProviderConfigurationNames.TrackerMaxConnection]);

            //StorageMaxConnection
            ValidateHelper.ShouldInt(result, Provider, FastDFSFileProviderConfigurationNames.StorageMaxConnection, keyValuePairs[FastDFSFileProviderConfigurationNames.StorageMaxConnection]);

            return result;
        }


    }
}
