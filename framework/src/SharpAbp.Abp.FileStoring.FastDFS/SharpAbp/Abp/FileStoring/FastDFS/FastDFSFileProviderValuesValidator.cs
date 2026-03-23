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

            //GroupName
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, FastDFSFileProviderConfigurationNames.GroupName, values.FindValue(FastDFSFileProviderConfigurationNames.GroupName));

            //Trackers
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, FastDFSFileProviderConfigurationNames.Trackers, values.FindValue(FastDFSFileProviderConfigurationNames.Trackers));

            //AntiStealCheckToken
            ValidateHelper.ShouldBool(result, Provider, FastDFSFileProviderConfigurationNames.AntiStealCheckToken, values.FindValue(FastDFSFileProviderConfigurationNames.AntiStealCheckToken));

            //DefaultTokenExpireSeconds
            ValidateHelper.ShouldInt(result, Provider, FastDFSFileProviderConfigurationNames.DefaultTokenExpireSeconds, values.FindValue(FastDFSFileProviderConfigurationNames.DefaultTokenExpireSeconds));

            //Charset
            ValidateHelper.NotNullOrWhiteSpace(result, Provider, FastDFSFileProviderConfigurationNames.Charset, values.FindValue(FastDFSFileProviderConfigurationNames.Charset));

            //NetworkTimeout
            ValidateHelper.ShouldInt(result, Provider, FastDFSFileProviderConfigurationNames.NetworkTimeout, values.FindValue(FastDFSFileProviderConfigurationNames.NetworkTimeout));

            //MaxConnectionPerServer
            ValidateHelper.ShouldInt(result, Provider, FastDFSFileProviderConfigurationNames.MaxConnectionPerServer, values.FindValue(FastDFSFileProviderConfigurationNames.MaxConnectionPerServer));

            //MinConnectionPerServer
            ValidateHelper.ShouldInt(result, Provider, FastDFSFileProviderConfigurationNames.MinConnectionPerServer, values.FindValue(FastDFSFileProviderConfigurationNames.MinConnectionPerServer));

            //ConnectionIdleTimeout
            ValidateHelper.ShouldInt(result, Provider, FastDFSFileProviderConfigurationNames.ConnectionIdleTimeout, values.FindValue(FastDFSFileProviderConfigurationNames.ConnectionIdleTimeout));

            //ConnectionLifeTime
            ValidateHelper.ShouldInt(result, Provider, FastDFSFileProviderConfigurationNames.ConnectionLifeTime, values.FindValue(FastDFSFileProviderConfigurationNames.ConnectionLifeTime));

            //ConnectionTimeout
            ValidateHelper.ShouldInt(result, Provider, FastDFSFileProviderConfigurationNames.ConnectionTimeout, values.FindValue(FastDFSFileProviderConfigurationNames.ConnectionTimeout));

            //SendTimeout
            ValidateHelper.ShouldInt(result, Provider, FastDFSFileProviderConfigurationNames.SendTimeout, values.FindValue(FastDFSFileProviderConfigurationNames.SendTimeout));

            //ReceiveTimeout
            ValidateHelper.ShouldInt(result, Provider, FastDFSFileProviderConfigurationNames.ReceiveTimeout, values.FindValue(FastDFSFileProviderConfigurationNames.ReceiveTimeout));

            return result;
        }
    }
}
