using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;

namespace Volo.Abp.FileStoring
{
    public class DefaultFileProviderSelector : IFileProviderSelector, ITransientDependency
    {
        protected  IEnumerable<IFileProvider> FileProviders { get; }

        protected IFileContainerConfigurationProvider ConfigurationProvider { get; }

        public DefaultFileProviderSelector(
            IFileContainerConfigurationProvider configurationProvider, 
            IEnumerable<IFileProvider> blobProviders)
        {
            ConfigurationProvider = configurationProvider;
            FileProviders = blobProviders;
        }
        
        [NotNull]
        public virtual IFileProvider Get([NotNull] string containerName)
        {
            Check.NotNull(containerName, nameof(containerName));
            
            var configuration = ConfigurationProvider.Get(containerName);
            
            if (!FileProviders.Any())
            {
                throw new AbpException("No BLOB Storage provider was registered! At least one provider must be registered to be able to use the Blog Storing System.");
            }
            
            foreach (var provider in FileProviders)
            {
                if (ProxyHelper.GetUnProxiedType(provider).IsAssignableTo(configuration.ProviderType))
                {
                    return provider;
                }
            }

            throw new AbpException(
                $"Could not find the BLOB Storage provider with the type ({configuration.ProviderType.AssemblyQualifiedName}) configured for the container {containerName} and no default provider was set."
            );
        }
    }
}