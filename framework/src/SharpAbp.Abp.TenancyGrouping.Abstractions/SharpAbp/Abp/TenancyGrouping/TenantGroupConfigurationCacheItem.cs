using System;
using Volo.Abp;

namespace SharpAbp.Abp.TenancyGrouping
{
    public class TenantGroupConfigurationCacheItem
    {
        private const string CacheKeyFormat = "i:{0},n:{1}";
        public TenantGroupConfiguration? Value { get; set; }

        public TenantGroupConfigurationCacheItem()
        {

        }

        public TenantGroupConfigurationCacheItem(TenantGroupConfiguration? value)
        {
            Value = value;
        }

        public static string CalculateCacheKey(Guid? id, string? name)
        {
            if (id == null && name.IsNullOrWhiteSpace())
            {
                throw new AbpException("Both id and name can't be invalid.");
            }
            return string.Format(CacheKeyFormat,
                id?.ToString() ?? "null",
                (name.IsNullOrWhiteSpace() ? "null" : name));
        }

        public static string CalculateCacheKey(Guid id)
        {
            return string.Format(CacheKeyFormat, id.ToString(), "null");
        }

        public static string CalculateCacheKey(string name)
        {
            return string.Format(CacheKeyFormat, "null", name);
        }
    }
}
