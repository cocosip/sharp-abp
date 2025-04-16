using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using SharpAbp.Abp.TenancyGrouping;
using SharpAbp.Abp.TenantGroupManagement.Localization;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupManager : DomainService, ITenantGroupManager
    {
        protected ILocalEventBus LocalEventBus { get; }
        protected ITenantGroupNormalizer TenantGroupNormalizer { get; }
        protected IStringLocalizer<TenantGroupManagementResource> Localizer { get; }
        protected ITenantGroupRepository TenantGroupRepository { get; }

        public TenantGroupManager(
            ILocalEventBus localEventBus,
            ITenantGroupNormalizer tenantGroupNormalizer,
            IStringLocalizer<TenantGroupManagementResource> localizer,
            ITenantGroupRepository tenantGroupRepository)
        {
            LocalEventBus = localEventBus;
            TenantGroupNormalizer = tenantGroupNormalizer;
            Localizer = localizer;
            TenantGroupRepository = tenantGroupRepository;
        }


        public virtual async Task<TenantGroup> CreateAsync(string name, bool isActive)
        {
            Check.NotNull(name, nameof(name));

            var normalizedName = TenantGroupNormalizer.NormalizeName(name);
            await ValidateNameAsync(normalizedName);
            return new TenantGroup(GuidGenerator.Create(), name, normalizedName, isActive);
        }


        public virtual async Task ChangeNameAsync(TenantGroup tenantGroup, string name, bool isActive)
        {
            Check.NotNull(tenantGroup, nameof(tenantGroup));
            Check.NotNull(name, nameof(name));

            var normalizedName = TenantGroupNormalizer.NormalizeName(name);

            await ValidateNameAsync(normalizedName, tenantGroup.Id);
            await LocalEventBus.PublishAsync(new TenantGroupChangedEvent(tenantGroup.Id, tenantGroup.NormalizedName));
            tenantGroup.SetName(name);
            tenantGroup.SetNormalizedName(normalizedName);
            tenantGroup.SetIsActive(isActive);
        }

        public virtual async Task<TenantGroup> AddTenantAsync(Guid id, Guid tenantId)
        {
            var tenantGroup = await TenantGroupRepository.GetAsync(id, true);
            ValidateTenant(tenantGroup, tenantId);
            tenantGroup.AddTenant(new TenantGroupTenant(GuidGenerator.Create(), tenantGroup.Id, tenantId));
            await LocalEventBus.PublishAsync(new TenantGroupChangedEvent(tenantGroup.Id, tenantGroup.NormalizedName, [.. tenantGroup.Tenants.Select(x => x.TenantId)]));
            return tenantGroup;
        }

        public virtual async Task<TenantGroup> RemoveTenantAsync(Guid id, Guid tenantGroupTenantId)
        {
            var tenantGroup = await TenantGroupRepository.GetAsync(id, true);
            tenantGroup.RemoveTenant(tenantGroupTenantId);
            await LocalEventBus.PublishAsync(new TenantGroupChangedEvent(tenantGroup.Id, tenantGroup.NormalizedName, [.. tenantGroup.Tenants.Select(x => x.TenantId)]));
            return tenantGroup;
        }


        protected virtual async Task ValidateNameAsync(string normalizeName, Guid? expectedId = null)
        {
            var tenantGroup = await TenantGroupRepository.FindExpectedByNameAsync(normalizeName, expectedId, false);
            if (tenantGroup != null)
            {
                throw new UserFriendlyException(Localizer["TenantGroupManagement.DuplicateName", normalizeName]).WithData("Name", normalizeName);
            }
        }

        protected virtual void ValidateTenant(TenantGroup tenantGroup, Guid tenantId)
        {
            if (tenantGroup.Tenants.Any(x => x.TenantId == tenantId))
            {
                throw new UserFriendlyException(Localizer["TenantGroupManagement.DuplicateTenantId", tenantId]);
            }
        }
    }
}
