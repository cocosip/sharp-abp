﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Reflection;

namespace SharpAbp.Abp.FileStoring
{
    public class AbpFileStoringOptions
    {
        public FileContainerConfigurations Containers { get; }

        public AbpFileStoringOptions()
        {
            Containers = new FileContainerConfigurations();
        }

        public AbpFileStoringOptions Configure(IConfiguration configuration, ServiceConfigurationContext context)
        {
            var entries = configuration
                .GetSection("FileStoringOptions")
                .Get<Dictionary<string, FileContainerConfigurationEntry>>();

            var abstractionsOptions = context.Services.ExecutePreConfiguredActions<AbpFileStoringAbstractionsOptions>();

            if (entries != null)
            {
                foreach (var entryKv in entries)
                {
                    var fileProviderConfiguration = abstractionsOptions.Providers.GetConfiguration(entryKv.Value.Provider);
                    if (fileProviderConfiguration == null)
                    {
                        throw new AbpException($"Could not find any provider configuration for '{entryKv.Key}' container, provider:'{entryKv.Value.Provider}'");
                    }

                    Containers.Configure(entryKv.Key, c =>
                    {
                        c.Provider = fileProviderConfiguration.Provider;
                        c.IsMultiTenant = entryKv.Value?.IsMultiTenant ?? false;
                        c.EnableAutoMultiPartUpload = entryKv.Value?.EnableAutoMultiPartUpload ?? false;
                        c.MultiPartUploadMinFileSize = entryKv.Value?.MultiPartUploadMinFileSize ?? 1024 * 1024 * 100;
                        c.MultiPartUploadShardingSize = entryKv.Value?.MultiPartUploadShardingSize ?? 1024 * 1024 * 3;
                        c.HttpAccess = entryKv.Value?.HttpAccess ?? true;

                        foreach (var defaultNamingNormalizer in fileProviderConfiguration.DefaultNamingNormalizers)
                        {
                            c.NamingNormalizers.Add(defaultNamingNormalizer);
                        }

                        var itemDict = fileProviderConfiguration.GetItems();

                        foreach (var itemKeyValuePair in itemDict)
                        {
                            entryKv.Value.Properties.TryGetValue(itemKeyValuePair.Key, out string value);

                            var item = itemKeyValuePair.Value;

                            var realValue = TypeHelper.ConvertFromString(item.ValueType, value);
                            c.SetConfiguration(itemKeyValuePair.Key, realValue);
                        }

                    });

                }
            }
            return this;
        }

    }
}