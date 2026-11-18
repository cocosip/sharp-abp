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
                    filePathBuilderEntry.ApplyTo(opts));
            }

            var entries = section.Get<Dictionary<string, FileContainerConfigurationBindingEntry>>();

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

                        // Only override values that were actually present in configuration so
                        // JSON entries inherit container/code defaults instead of resetting them.
                        if (entry.IsMultiTenant.HasValue)
                        {
                            c.IsMultiTenant = entry.IsMultiTenant.Value;
                        }

                        if (entry.EnableAutoMultiPartUpload.HasValue)
                        {
                            c.EnableAutoMultiPartUpload = entry.EnableAutoMultiPartUpload.Value;
                        }

                        if (entry.MultiPartUploadMinFileSize.HasValue)
                        {
                            c.MultiPartUploadMinFileSize = entry.MultiPartUploadMinFileSize.Value;
                        }

                        if (entry.MultiPartUploadShardingSize.HasValue)
                        {
                            c.MultiPartUploadShardingSize = entry.MultiPartUploadShardingSize.Value;
                        }

                        if (entry.HttpAccess.HasValue)
                        {
                            c.HttpAccess = entry.HttpAccess.Value;
                        }

                        foreach (var defaultNamingNormalizer in fileProviderConfiguration.DefaultNamingNormalizers)
                        {
                            c.NamingNormalizers.Add(defaultNamingNormalizer);
                        }

                        var itemPairs = fileProviderConfiguration.GetItems();
                        foreach (var key in itemPairs.Keys)
                        {
                            if (entry.Properties != null && entry.Properties.ContainsKey(key) && itemPairs[key] != null)
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

        /// <summary>
        /// Binding model that keeps track of which values were actually present in configuration.
        /// </summary>
        internal sealed class FileContainerConfigurationBindingEntry
        {
            public string? Provider { get; set; }
            public bool? IsMultiTenant { get; set; }
            public bool? EnableAutoMultiPartUpload { get; set; }
            public int? MultiPartUploadMinFileSize { get; set; }
            public int? MultiPartUploadShardingSize { get; set; }
            public bool? HttpAccess { get; set; }
            public Dictionary<string, string> Properties { get; set; } = [];
        }
    }
}
