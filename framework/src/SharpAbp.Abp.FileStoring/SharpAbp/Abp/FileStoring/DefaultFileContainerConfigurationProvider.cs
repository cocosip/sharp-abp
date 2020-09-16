using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring
{
    public class DefaultFileContainerConfigurationProvider : IFileContainerConfigurationProvider, ITransientDependency
    {
        protected AbpFileStoringOptions Options { get; }

        public DefaultFileContainerConfigurationProvider(IOptions<AbpFileStoringOptions> options)
        {
            Options = options.Value;
        }

        public virtual FileContainerConfiguration Get(string name)
        {
            return Options.Containers.GetConfiguration(name);
        }

        /// <summary>
        /// Get many configuration
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>The configuration list</returns>
        public virtual List<FileContainerConfiguration> GetList(Func<FileContainerConfiguration, bool> predicate)
        {
            return Options.Containers.GetConfigurations(predicate);
        }
    }
}