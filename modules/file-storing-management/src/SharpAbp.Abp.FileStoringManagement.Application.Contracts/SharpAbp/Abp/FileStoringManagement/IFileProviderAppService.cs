﻿using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Application service interface for managing file storing providers.
    /// Provides functionality to discover available providers and retrieve their configuration options.
    /// </summary>
    public interface IFileProviderAppService : IApplicationService
    {
        /// <summary>
        /// Retrieves all available file storing providers in the system.
        /// </summary>
        /// <returns>A list of provider information containing provider names and configurations.</returns>
        Task<List<ProviderDto>> GetProvidersAsync();

        /// <summary>
        /// Checks whether a specific file storing provider exists in the system.
        /// </summary>
        /// <param name="provider">The name of the provider to check. Cannot be null or whitespace.</param>
        /// <returns>True if the provider exists; otherwise, false.</returns>
        Task<bool> HasProviderAsync([NotNull] string provider);

        /// <summary>
        /// Retrieves the configuration options available for a specific file storing provider.
        /// </summary>
        /// <param name="provider">The name of the provider to get options for. Cannot be null or whitespace.</param>
        /// <returns>The provider options including all available configuration parameters with their types, examples, and localized descriptions.</returns>
        Task<ProviderOptionsDto> GetOptionsAsync([NotNull] string provider);
    }
}
