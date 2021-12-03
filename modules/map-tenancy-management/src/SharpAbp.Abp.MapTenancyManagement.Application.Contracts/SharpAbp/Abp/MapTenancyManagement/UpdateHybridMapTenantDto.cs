using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class UpdateHybridMapTenantDto : TenantCreateOrUpdateDtoBase
    {
        public string Code { get; set; }
        public string MapCode { get; set; }
    }
}