using SharpAbp.Abp.MapTenancy;

namespace SharpAbp.Abp.AspNetCore.MapTenancy
{
    public class AbpAspNetCoreMapTenancyOptions
    {
        /// <summary>
        /// Default: <see cref="MapTenantResolverConsts.DefaultMapTenantKey"/>.
        /// </summary>
        public string MapTenantKey { get; set; }

        public AbpAspNetCoreMapTenancyOptions()
        {
            MapTenantKey = MapTenantResolverConsts.DefaultMapTenantKey;
        }
    }
}
