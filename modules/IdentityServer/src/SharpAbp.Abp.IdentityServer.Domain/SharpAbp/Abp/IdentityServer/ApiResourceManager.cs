using Microsoft.Extensions.Localization;
using SharpAbp.Abp.IdentityServer.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.IdentityServer.ApiResources;

namespace SharpAbp.Abp.IdentityServer
{
    public class ApiResourceManager : DomainService
    {
        protected IStringLocalizer<IdentityServerResource> Localizer { get; }
        protected IApiResourceRepository ApiResourceRepository { get; }
        public ApiResourceManager(
            IStringLocalizer<IdentityServerResource> localizer,
            IApiResourceRepository apiResourceRepository)
        {
            Localizer = localizer;
            ApiResourceRepository = apiResourceRepository;
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="apiResource"></param>
        /// <returns></returns>
        public virtual async Task CreateAsync(ApiResource apiResource)
        {
            await ValidateApiSourceNameAsync(apiResource.Name);

            await ApiResourceRepository.InsertAsync(apiResource);
        }


        /// <summary>
        /// Validate apiResource name
        /// </summary>
        /// <param name="apiResourceName"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual async Task ValidateApiSourceNameAsync(
            string apiResourceName,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            var apiResource = await ApiResourceRepository.FindByNameAsync(
                apiResourceName,
                includeDetails,
                cancellationToken);

            if (apiResource != null)
            {
                throw new UserFriendlyException(Localizer["IdentityServer.DumplicateApiResourceName", apiResourceName]);
            }
        }


    }
}
