using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.FileStoring
{
    public interface IFileContainerConfigurationProvider
    {
        /// <summary>
        /// Gets a <see cref="FileContainerConfiguration"/> for the given container <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the container</param>
        /// <returns>The configuration that should be used for the container</returns>
        FileContainerConfiguration Get(string name);

        /// <summary>
        /// Get many configuration
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>The configuration list</returns>
        List<FileContainerConfiguration> GetList(Func<FileContainerConfiguration, bool> predicate);
    }
}