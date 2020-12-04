using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Reflection;

namespace SharpAbp.Abp.FileStoring
{
    public class AbpFileStoringOptions
    {
        public FileContainerConfigurations Containers { get; }
        public FileProviderConfigurations Providers { get; }

        public AbpFileStoringOptions()
        {
            Containers = new FileContainerConfigurations();
            Providers = new FileProviderConfigurations();
        }

        public AbpFileStoringOptions Configure(IConfiguration configuration)
        {
            var providerConfigurations = configuration.Get<Dictionary<string, ProviderConfiguration>>();

            foreach (var kv in providerConfigurations)
            {
                var fileProviderConfiguration = Providers.GetConfiguration(kv.Value.Provider);
                if (fileProviderConfiguration == null)
                {
                    throw new AbpException($"Could not find any provider configuration for '{kv.Key}' container, provider:'{kv.Value.Provider}'");
                }

                Containers.Configure(kv.Key, c =>
                {
                    c.Provider = fileProviderConfiguration.Provider;
                    c.IsMultiTenant = kv.Value?.IsMultiTenant ?? false;
                    c.HttpSupport = kv.Value?.HttpSupport ?? true;

                    foreach (var defaultNamingNormalizer in fileProviderConfiguration.DefaultNamingNormalizers)
                    {
                        c.NamingNormalizers.Add(defaultNamingNormalizer);
                    }

                    var defaultProperties = fileProviderConfiguration.GetProperties();

                    foreach (var defaultProperty in defaultProperties)
                    {
                        kv.Value.Properties.TryGetValue(defaultProperty.Key, out string value);
                        var realValue = TypeHelper.ConvertFromString(defaultProperty.Value, value);
                        c.SetConfiguration(defaultProperty.Key, realValue);
                    }

                });

            }

            return this;
        }

    }
}