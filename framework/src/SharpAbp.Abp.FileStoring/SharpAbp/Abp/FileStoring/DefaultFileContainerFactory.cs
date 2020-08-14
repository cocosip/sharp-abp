using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Net.Http.Headers;

namespace SharpAbp.Abp.FileStoring
{
    public class DefaultFileContainerFactory : IFileContainerFactory
    {
        private readonly ILogger _logger;

        private readonly ConcurrentDictionary<Guid, IFileContainer> _fileContainerDict;

        public DefaultFileContainerFactory(ILogger<DefaultFileContainerFactory> logger)
        {
            _logger = logger;
            _fileContainerDict = new ConcurrentDictionary<Guid, IFileContainer>();
        }

        public IFileContainer GetOrCreateContainer(FileContainerConfiguration configuration)
        {
            return default;
        }




    }
}
