﻿using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.FileStoring;
using System;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Reflection;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class DefaultFileContainerConfigurationConverter : IFileContainerConfigurationConverter, ITransientDependency
    {
        protected AbpFileStoringOptions Options { get; }
        protected IServiceProvider ServiceProvider { get; }

        public DefaultFileContainerConfigurationConverter(IServiceProvider serviceProvider, IOptions<AbpFileStoringOptions> options)
        {
            ServiceProvider = serviceProvider;
            Options = options.Value;
        }

        /// <summary>
        /// Convert database entity 'FileStoringContainer' to 'FileContainerConfiguration'
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        [NotNull]
        public virtual FileContainerConfiguration ToConfiguration(FileStoringContainer container)
        {
            var fileProviderConfiguration = Options.Providers.GetConfiguration(container.ProviderName);
            Check.NotNull(fileProviderConfiguration, nameof(fileProviderConfiguration));

            var configuration = new FileContainerConfiguration()
            {
                Provider = fileProviderConfiguration.Provider,
                IsMultiTenant = !container.TenantId.HasValue,
                HttpSupport = container.HttpSupport
            };

            foreach (var item in container.Items)
            {
                var type = Type.GetType(item.TypeName);
                var value = TypeHelper.ConvertFromString(type, item.Value);
                configuration.SetConfiguration(item.Name, value);
            }


            foreach (var namingNormalizer in fileProviderConfiguration.DefaultNamingNormalizers)
            {
                configuration.NamingNormalizers.Add(namingNormalizer);
            }

            return configuration;
        }

    }
}