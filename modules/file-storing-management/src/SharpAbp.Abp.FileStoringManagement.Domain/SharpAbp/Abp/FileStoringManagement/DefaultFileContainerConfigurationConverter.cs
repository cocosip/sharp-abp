using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.FileStoring;
using System;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

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
            var providerType = Type.GetType(container.ProviderTypeName);
            Check.NotNull(providerType, nameof(providerType));

            var configuration = new FileContainerConfiguration()
            {
                ProviderType = providerType,
                IsMultiTenant = !container.TenantId.HasValue,
                HttpSupport = container.HttpSupport
            };

            foreach (var item in container.Items)
            {
                var value = ConvertType(item.Value, item.TypeName);
                configuration.SetConfiguration(item.Name, value);
            }

            //NamingNormalizers
            var fileProviderConfiguration = Options.Providers.GetConfiguration(providerType);
            if (fileProviderConfiguration != null)
            {
                foreach (var namingNormalizer in fileProviderConfiguration.DefaultNamingNormalizers)
                {
                    configuration.NamingNormalizers.Add(namingNormalizer);
                }
            }

            return configuration;
        }



        protected virtual object ConvertType(string value, string typeName)
        {
            var parameterType = Type.GetType(typeName);
            if (parameterType == typeof(string))
            {
                return value;
            }

            if (parameterType.IsPrimitive)
            {
                return Convert.ChangeType(value, parameterType);
            }

            var typeConverters = ServiceProvider.GetServices<IFileContainerConfigurationTypeConverter>();
            foreach (var typeConverter in typeConverters)
            {
                if (typeConverter.TargetType == parameterType)
                {
                    return typeConverter.ConvertType(value);
                }
            }

            throw new UserFriendlyException($"Can't change 'string' to '{parameterType}',because can't find any 'IFileContainerConfigurationTypeConverter'.", "404");

        }

    }
}
