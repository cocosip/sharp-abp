using OBS;
using System;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.Obs
{
    public class DefaultObsClientFactory : IObsClientFactory, ITransientDependency
    {

        public virtual ObsClient Create(ObsFileProviderConfiguration configuration)
        {
            Check.NotNullOrWhiteSpace(configuration.AccessKeyId, nameof(configuration.AccessKeyId));
            Check.NotNullOrWhiteSpace(configuration.AccessKeySecret, nameof(configuration.AccessKeySecret));
            Check.NotNullOrWhiteSpace(configuration.Endpoint, nameof(configuration.Endpoint));

            var obsConfig = new ObsConfig
            {
                Endpoint = configuration.Endpoint,
                AuthType = GetAuthType(configuration.AuthType),
                AuthTypeNegotiation = configuration.AuthTypeNegotiation,
            };

            return new ObsClient(configuration.AccessKeyId, configuration.AccessKeySecret, obsConfig);
        }

        protected virtual AuthTypeEnum GetAuthType(string authType)
        {
            foreach (var name in Enum.GetNames(typeof(AuthTypeEnum)))
            {
                if (string.Equals(name, authType, StringComparison.OrdinalIgnoreCase))
                {
                    return (AuthTypeEnum)Enum.Parse(typeof(AuthTypeEnum), name);
                }
            }

            throw new AbpException($"Invalid Obs authentication type '{authType}'. Allowed values: {string.Join(", ", Enum.GetNames(typeof(AuthTypeEnum)))}.");
        }
    }
}
