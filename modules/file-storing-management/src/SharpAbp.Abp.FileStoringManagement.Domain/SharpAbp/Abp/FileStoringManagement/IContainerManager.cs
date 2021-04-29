using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace SharpAbp.Abp.FileStoringManagement
{
    public interface IContainerManager : IDomainService
    {
        /// <summary>
        /// Validate provider values
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="keyValuePairs"></param>
        void ValidateProviderValues(string provider, Dictionary<string, string> keyValuePairs);

        /// <summary>
        /// Validate container name
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="name"></param>
        /// <param name="expectedId"></param>
        /// <returns></returns>
        Task ValidateNameAsync(Guid? tenantId, string name, Guid? expectedId = null);
    }
}
