using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Volo.Abp;

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
            var providerConfigurationEntries = configuration.Get<Dictionary<string, ProviderConfigurationEntrty>>();

            foreach (var kv in providerConfigurationEntries)
            {
                var fileProviderConfiguration = Providers.GetConfiguration(kv.Value.Provider);
                if (fileProviderConfiguration == null)
                {
                    throw new AbpException($"Could not find any provider configuration for '{kv.Key}' container, provider:'{kv.Value.Provider}'");
                }

                Containers.Configure(kv.Key, c =>
                {
                    c.ProviderType = fileProviderConfiguration.ProviderType;
                    c.IsMultiTenant = kv.Value?.IsMultiTenant ?? false;
                    c.HttpSupport = kv.Value?.HttpSupport ?? true;

                    foreach (var defaultNamingNormalizer in fileProviderConfiguration.DefaultNamingNormalizers)
                    {
                        c.NamingNormalizers.Add(defaultNamingNormalizer);
                    }

                    foreach (var property in kv.Value.Properties)
                    {

                        var providerProperyType = fileProviderConfiguration.GetProperty(property.Key);
                        if (providerProperyType != null)
                        {
                            var value = FileStoringUtil.ConvertPrimitiveType(property.Value, providerProperyType, true);
                            c.SetConfiguration(property.Key, value);
                        }
                        else
                        {
                            c.SetConfiguration(property.Key, property.Value);
                        }
                    }

                });

            }

            return this;
        }

    }
}