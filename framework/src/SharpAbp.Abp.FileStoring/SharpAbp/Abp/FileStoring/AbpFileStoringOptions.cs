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
            var entries = configuration
                .GetSection("FileStoringOptions")
                .Get<Dictionary<string, FileContainerConfigurationEntry>>();
            if (entries != null)
            {
                foreach (var entryKv in entries)
                {
                    var fileProviderConfiguration = Providers.GetConfiguration(entryKv.Value.Provider);
                    if (fileProviderConfiguration == null)
                    {
                        throw new AbpException($"Could not find any provider configuration for '{entryKv.Key}' container, provider:'{entryKv.Value.Provider}'");
                    }

                    Containers.Configure(entryKv.Key, c =>
                    {
                        c.Provider = fileProviderConfiguration.Provider;
                        c.IsMultiTenant = entryKv.Value?.IsMultiTenant ?? false;
                        c.HttpAccess = entryKv.Value?.HttpAccess ?? true;

                        foreach (var defaultNamingNormalizer in fileProviderConfiguration.DefaultNamingNormalizers)
                        {
                            c.NamingNormalizers.Add(defaultNamingNormalizer);
                        }

                        var valueTypes = fileProviderConfiguration.GetValueTypes();

                        foreach (var valueTypeKv in valueTypes)
                        {
                            entryKv.Value.Properties.TryGetValue(valueTypeKv.Key, out string value);
                            var realValue = TypeHelper.ConvertFromString(valueTypeKv.Value, value);
                            c.SetConfiguration(valueTypeKv.Key, realValue);
                        }

                    });

                }
            }
            return this;
        }

    }
}