using JetBrains.Annotations;
using Microsoft.Extensions.Localization;
using SharpAbp.Abp.FileStoring;
using SharpAbp.Abp.FileStoringManagement.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class ContainerManager : DomainService
    {
        protected IStringLocalizer<FileStoringManagementResource> Localizer { get; }
        protected IEnumerable<IFileProviderValuesValidator> ProviderValuesValidators { get; }
        protected IFileStoringContainerRepository ContainerRepository { get; }
        public ContainerManager(
            IStringLocalizer<FileStoringManagementResource> localizer,
            IEnumerable<IFileProviderValuesValidator> providerValuesValidators,
            IFileStoringContainerRepository containerRepository)
        {
            Localizer = localizer;
            ProviderValuesValidators = providerValuesValidators;
            ContainerRepository = containerRepository;
        }

        /// <summary>
        /// Validate provider values
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="keyValuePairs"></param>
        public virtual void ValidateProviderValues(string provider, Dictionary<string, string> keyValuePairs)
        {
            var valuesValidator = GetFileProviderValuesValidator(provider);
            if (valuesValidator != null)
            {
                var result = valuesValidator.Validate(keyValuePairs);
                if (result.Errors.Any())
                {
                    throw new AbpValidationException(Localizer["FileStoringManagement.ValidateContainerFailed", provider], result.Errors);
                }
            }
        }

        /// <summary>
        /// Validate container name
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="name"></param>
        /// <param name="expectedId"></param>
        /// <returns></returns>
        public virtual async Task ValidateNameAsync(
            Guid? tenantId,
            string name,
            Guid? expectedId = null)
        {
            using (CurrentTenant.Change(tenantId))
            {
                var container = await ContainerRepository.FindExpectedByNameAsync(name, expectedId, false);
                if (container != null)
                {
                    throw new UserFriendlyException(Localizer["FileStoringManagement.DuplicateContainerName", name]);
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
            return null;
        }

    }
}
