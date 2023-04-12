using Microsoft.Extensions.Localization;
using SharpAbp.Abp.TenantGroupManagement.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupManager : DomainService
    {
        protected ILocalEventBus LocalEventBus { get; }
        protected IStringLocalizer<TenantGroupManagementResource> Localizer { get; }
        protected ITenantGroupRepository TenantGroupRepository { get; }

        public TenantGroupManager(
            ILocalEventBus localEventBus,
            IStringLocalizer<TenantGroupManagementResource> localizer,
            ITenantGroupRepository tenantGroupRepository)
        {
            LocalEventBus = localEventBus;
            Localizer = localizer;
            TenantGroupRepository = tenantGroupRepository;
        }


        public virtual async Task<TenantGroup> CreateAsync(TenantGroup tenantGroup)
        {
            await ValidateNameAsync(tenantGroup.Name);
            return await TenantGroupRepository.InsertAsync(tenantGroup);
        }

        public virtual async Task<TenantGroup> UpdateAsync(Guid id, string name, bool isActive)
        {
            await ValidateNameAsync(name, id);
            var tenantGroup = await TenantGroupRepository.GetAsync(id);
            tenantGroup.SetName(name);
            tenantGroup.IsActive = isActive;
            return await TenantGroupRepository.UpdateAsync(tenantGroup);
        }


        public virtual async Task<TenantGroup> AddTenantAsync(Guid id, Guid tenantId)
        {
            var tenantGroup = await TenantGroupRepository.GetAsync(id, true);
            tenantGroup.AddTenant(new TenantGroupTenant(GuidGenerator.Create(), tenantGroup.Id, tenantId));
            return await TenantGroupRepository.UpdateAsync(tenantGroup);
        }

        public virtual async Task<TenantGroup> RemoveTenantAsync(Guid id, Guid tenantGroupTenantId)
        {
            var tenantGroup = await TenantGroupRepository.GetAsync(id, true);
            tenantGroup.RemoveTenant(tenantGroupTenantId);
            return await TenantGroupRepository.UpdateAsync(tenantGroup);
        }


        protected virtual async Task ValidateNameAsync(string name, Guid? expectedId = null)
        {
            var tenantGroup = await TenantGroupRepository.FindExpectedByNameAsync(name, expectedId, false);
            if (tenantGroup != null)
            {
                throw new UserFriendlyException(Localizer["TenantGroupManagement.DuplicateName", name]);
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
