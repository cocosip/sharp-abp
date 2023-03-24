using SharpAbp.Abp.TenancyGrouping;
using System;
using Volo.Abp;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.TenantGroupManagement
{

    [Serializable]
    [IgnoreMultiTenancy]
    public class TenantGroupCacheItem
    {
        private const string CacheKeyFormat = "i:{0},n:{1}";

        public TenantGroupConfiguration Value { get; set; }

        public TenantGroupCacheItem()
        {

        }

        public TenantGroupCacheItem(TenantGroupConfiguration value)
        {
            Value = value;
        }

        public static string CalculateCacheKey(Guid? id, string name)
        {
            if (id == null && name.IsNullOrWhiteSpace())
            {
                throw new AbpException("Both id and name can't be invalid.");
            }

            return string.Format(CacheKeyFormat,
                id?.ToString() ?? "null",
                (name.IsNullOrWhiteSpace() ? "null" : name));
        }
    }
}
