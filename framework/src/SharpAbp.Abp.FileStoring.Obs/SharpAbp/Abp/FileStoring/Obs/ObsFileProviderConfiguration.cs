using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring.Obs
{
    public class ObsFileProviderConfiguration
    {
        public string AccessKeyId
        {
            get => _containerConfiguration.GetConfiguration<string>(ObsFileProviderConfigurationNames.AccessKeyId);
            set => _containerConfiguration.SetConfiguration(ObsFileProviderConfigurationNames.AccessKeyId, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        public string AccessKeySecret
        {
            get => _containerConfiguration.GetConfiguration<string>(ObsFileProviderConfigurationNames.AccessKeySecret);
            set => _containerConfiguration.SetConfiguration(ObsFileProviderConfigurationNames.AccessKeySecret, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        public string Endpoint
        {
            get => _containerConfiguration.GetConfiguration<string>(ObsFileProviderConfigurationNames.Endpoint);
            set => _containerConfiguration.SetConfiguration(ObsFileProviderConfigurationNames.Endpoint, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        private readonly FileContainerConfiguration _containerConfiguration;

        public ObsFileProviderConfiguration(FileContainerConfiguration containerConfiguration)
        {
            _containerConfiguration = containerConfiguration;
            //_temporaryCredentialsCacheKey = Guid.NewGuid().ToString("N");
        }
    }
}
