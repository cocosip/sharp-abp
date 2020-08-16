using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    public class FileContainerConfigurations
    {
        private readonly Dictionary<string, FileContainerConfiguration> _containers;

        public FileContainerConfigurations()
        {
            _containers = new Dictionary<string, FileContainerConfiguration>
            {
            };
        }


        public FileContainerConfigurations Configure(
                 [NotNull] string name,
                 [NotNull] Action<FileContainerConfiguration> configureAction)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(configureAction, nameof(configureAction));

            configureAction(
                _containers.GetOrAdd(
                    name,
                    () => new FileContainerConfiguration()
                )
            );

            return this;
        }


        [NotNull]
        public FileContainerConfiguration GetConfiguration([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            return _containers.GetOrDefault(name);
        }

    }
}
