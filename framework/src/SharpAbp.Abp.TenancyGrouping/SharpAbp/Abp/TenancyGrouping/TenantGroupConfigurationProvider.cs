using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using SharpAbp.Abp.TenancyGrouping.Localization;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.TenancyGrouping
{
    public class TenantGroupConfigurationProvider : ITenantGroupConfigurationProvider, ITransientDependency
    {
        protected virtual ITenantGroupResolver TenantGroupResolver { get; }
        protected virtual ITenantGroupStore TenantGroupStore { get; }
        protected virtual ITenantGroupNormalizer TenantGroupNormalizer { get; }
        protected virtual ITenantGroupResolveResultAccessor TenantResolveResultAccessor { get; }
        protected virtual IStringLocalizer<AbpTenancyGroupingResource> StringLocalizer { get; }

        public TenantGroupConfigurationProvider(
            ITenantGroupResolver tenantGroupResolver,
            ITenantGroupStore tenantGroupStore,
            ITenantGroupResolveResultAccessor tenantGroupResolveResultAccessor,
            IStringLocalizer<AbpTenancyGroupingResource> stringLocalizer,
            ITenantGroupNormalizer tenantGroupNormalizer)
        {
            TenantGroupResolver = tenantGroupResolver;
            TenantGroupStore = tenantGroupStore;
            TenantGroupNormalizer = tenantGroupNormalizer;
            TenantResolveResultAccessor = tenantGroupResolveResultAccessor;
            StringLocalizer = stringLocalizer;
        }

        public virtual async Task<TenantGroupConfiguration?> GetAsync(bool saveResolveResult = false)
        {
            var resolveResult = await TenantGroupResolver.ResolveGroupIdOrNameAsync();

            if (saveResolveResult)
            {
                TenantResolveResultAccessor.Result = resolveResult;
            }

            TenantGroupConfiguration? tenant = null;
            if (resolveResult.GroupIdOrName != null)
            {
                tenant = await FindTenantGroupAsync(resolveResult.GroupIdOrName);

                if (tenant == null)
                {
                    throw new BusinessException(
                        code: "Volo.AbpIo.TenancyGrouping:010001",
                        message: StringLocalizer["TenantGroupNotFoundMessage"],
                        details: StringLocalizer["TenantGroupNotFoundDetails", resolveResult.GroupIdOrName]
                    );
                }

                if (!tenant.IsActive)
                {
                    throw new BusinessException(
                        code: "Volo.AbpIo.TenancyGrouping:010002",
                        message: StringLocalizer["TenantGroupNotActiveMessage"],
                        details: StringLocalizer["TenantGroupNotActiveDetails", resolveResult.GroupIdOrName]
                    );
                }
            }

            return tenant;
        }

        protected virtual async Task<TenantGroupConfiguration?> FindTenantGroupAsync(string groupIdOrName)
        {
            if (Guid.TryParse(groupIdOrName, out var parsedTenantId))
            {
                return await TenantGroupStore.FindAsync(parsedTenantId);
            }
            else
            {
                return await TenantGroupStore.FindAsync(TenantGroupNormalizer.NormalizeName(groupIdOrName)!);
            }
        }
    }
}
