using System;
using JetBrains.Annotations;
using Volo.Abp;

namespace SharpAbp.Abp.TenancyGrouping
{
    public static class CurrentTenantGroupExtensions
    {
        public static Guid GetId([NotNull] this ICurrentTenantGroup currentTenantGroup)
        {
            Check.NotNull(currentTenantGroup, nameof(currentTenantGroup));

            if (currentTenantGroup.Id == null)
            {
                throw new AbpException("Current Tenant group Id is not available!");
            }

            return currentTenantGroup.Id.Value;
        }

 
    }
}
