using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    public class FileContainerConfigurations
    {
        private FileContainerConfiguration Default => GetConfiguration<DefaultContainer>();

        private readonly Dictionary<string, FileContainerConfiguration> _containers;

        public FileContainerConfigurations()
        {
            _containers = new Dictionary<string, FileContainerConfiguration>
            {
                //Add default container
                [FileContainerNameAttribute.GetContainerName<DefaultContainer>()] = new FileContainerConfiguration()
            };
        }

        public FileContainerConfigurations Configure<TContainer>(
            Action<FileContainerConfiguration> configureAction)
        {
            return Configure(
                FileContainerNameAttribute.GetContainerName<TContainer>(),
                configureAction
            );
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
                    () => new FileContainerConfiguration(Default)
                )
            );

            return this;
        }

        public FileContainerConfigurations ConfigureDefault(Action<FileContainerConfiguration> configureAction)
        {
            configureAction(Default);
            return this;
        }

        public FileContainerConfigurations ConfigureAll(Action<string, FileContainerConfiguration> configureAction)
        {
            foreach (var container in _containers)
            {
                configureAction(container.Key, container.Value);
            }

            return this;
        }

        [NotNull]
        public FileContainerConfiguration GetConfiguration<TContainer>()
        {
            return GetConfiguration(FileContainerNameAttribute.GetContainerName<TContainer>());
        }

        [NotNull]
        public FileContainerConfiguration GetConfiguration([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            return _containers.GetOrDefault(name) ??
                   Default;
        }

        public List<FileContainerConfiguration> GetConfigurations([NotNull] Func<FileContainerConfiguration, bool> predicate)
        {
            return _containers.Values.Where(predicate).ToList();
        }
    }
}