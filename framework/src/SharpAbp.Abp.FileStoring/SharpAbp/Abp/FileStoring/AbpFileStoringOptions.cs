using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            var section = configuration.GetSection("FileStoringOptions");

            // Apply FilePathBuilder configuration if present
            var filePathBuilderEntry = section.GetSection("FilePathBuilder").Get<FilePathBuilderEntry>();
            if (filePathBuilderEntry != null)
            {
                context.Services.Configure<AbpFileStoringAbstractionsOptions>(opts =>
                {
                    if (filePathBuilderEntry.FilePathStrategy.HasValue)
                    {
                        opts.FilePathStrategy = filePathBuilderEntry.FilePathStrategy.Value;
                    }

                    if (!string.IsNullOrEmpty(filePathBuilderEntry.Prefix))
                    {
                        opts.FilePathBuilder.Prefix = filePathBuilderEntry.Prefix;
                    }

                    if (filePathBuilderEntry.HostSegment != null)
                    {
                        opts.FilePathBuilder.HostSegment = filePathBuilderEntry.HostSegment;
                    }

                    if (filePathBuilderEntry.TenantsSegment != null)
                    {
                        opts.FilePathBuilder.TenantsSegment = filePathBuilderEntry.TenantsSegment;
                    }

                    if (filePathBuilderEntry.TenantIdentifierMode.HasValue)
                    {
                        if (filePathBuilderEntry.TenantIdentifierMode.Value == TenantIdentifierMode.TenantName)
                        {
                            opts.FilePathBuilder.TenantIdentifierFactory = (id, name, ctx) =>
                                ctx?.TenantCode
                                ?? (!string.IsNullOrEmpty(name) ? name! : id.ToString("D"));
                        }
                        else
                        {
                            // TenantId mode: clear any previously set factory → DefaultFilePathBuilder uses GUID
                            opts.FilePathBuilder.TenantIdentifierFactory = null;
                        }
                    }
                });
            }

            var entries = section.Get<Dictionary<string, FileContainerConfigurationEntry>>();

            var abstractionsOptions = context.Services.ExecutePreConfiguredActions<AbpFileStoringAbstractionsOptions>();

            if (entries != null)
            {
                foreach (var entryKv in entries)
                {
                    // Skip the reserved "FilePathBuilder" key
                    if (string.Equals(entryKv.Key, "FilePathBuilder", System.StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var entry = entryKv.Value!;

                    var fileProviderConfiguration = abstractionsOptions.Providers.GetConfiguration(entry.Provider!);
                    if (fileProviderConfiguration == null)
                    {
                        throw new AbpException($"Could not find any provider configuration for '{entryKv.Key}' container, provider:'{entry.Provider}'");
                    }

                    Containers.Configure(entryKv.Key, c =>
                    {
                        c.Provider = fileProviderConfiguration.Provider;
                        c.IsMultiTenant = entry.IsMultiTenant;
                        c.EnableAutoMultiPartUpload = entry.EnableAutoMultiPartUpload;
                        c.MultiPartUploadMinFileSize = entry.MultiPartUploadMinFileSize;
                        c.MultiPartUploadShardingSize = entry.MultiPartUploadShardingSize;
                        c.HttpAccess = entry.HttpAccess;

                        foreach (var defaultNamingNormalizer in fileProviderConfiguration.DefaultNamingNormalizers)
                        {
                            c.NamingNormalizers.Add(defaultNamingNormalizer);
                        }

                        var itemPairs = fileProviderConfiguration.GetItems();
                        foreach (var key in itemPairs.Keys)
                        {
                            if (entry.Properties.ContainsKey(key) && itemPairs[key] != null)
                            {
                                var o = TypeHelper.ConvertFromString(itemPairs[key].ValueType!, entry.Properties[key]);
                                c.SetConfiguration(key, o);
                            }
                        }
                    });
                }
            }
            return this;
        }

    }
}