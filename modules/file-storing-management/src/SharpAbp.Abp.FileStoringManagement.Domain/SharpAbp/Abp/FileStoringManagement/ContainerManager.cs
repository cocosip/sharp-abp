using JetBrains.Annotations;
using SharpAbp.Abp.FileStoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class ContainerManager : DomainService
    {
        protected IEnumerable<IFileProviderValuesValidator> ProviderValuesValidators { get; }
        protected IFileStoringContainerRepository FileStoringContainerRepository { get; }
        public ContainerManager(
            IEnumerable<IFileProviderValuesValidator> providerValuesValidators,
            IFileStoringContainerRepository fileStoringContainerRepository)
        {
            ProviderValuesValidators = providerValuesValidators;
            FileStoringContainerRepository = fileStoringContainerRepository;
        }

        /// <summary>
        /// Validate provider values
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="keyValuePairs"></param>
        public virtual void ValidateProviderValues(string provider, Dictionary<string, string> keyValuePairs)
        {
            var valuesValidator = GetFileProviderValuesValidator(provider);

            var result = valuesValidator.Validate(keyValuePairs);
            if (result.Errors.Any())
            {
                throw new AbpValidationException("Create Container validate failed.", result.Errors);
            }
        }

        /// <summary>
        /// Validate container name
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="name"></param>
        /// <param name="expectedId"></param>
        /// <returns></returns>
        public virtual async Task ValidateNameAsync(Guid? tenantId, string name, Guid? expectedId = null)
        {
            using (CurrentTenant.Change(tenantId))
            {
                var container = await FileStoringContainerRepository.FindExpectedByNameAsync(name, expectedId, false);
                if (container != null)
                {
                    throw new AbpException($"Duplicate container name '{name}' in tenant '{tenantId}'.");
                }
            }
        }

        protected virtual IFileProviderValuesValidator GetFileProviderValuesValidator([NotNull] string provider)
        {
            Check.NotNullOrWhiteSpace(provider, nameof(provider));

            foreach (var providerValuesValidator in ProviderValuesValidators)
            {
                if (providerValuesValidator.Provider == provider)
                {
                    return providerValuesValidator;
                }
            }
            throw new AbpException($"Could not find any 'IFileProviderValuesValidator' for provider '{provider}' .");
        }

    }
}
