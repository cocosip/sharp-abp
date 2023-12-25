using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.TenantManagement;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class CreateMapTenantDto
    {
        [Required]
        public Guid TenantId { get; set; }

        [Required]
        [DynamicStringLength(typeof(TenantConsts), nameof(TenantConsts.MaxNameLength))]
        public string TenantName { get; set; }

        [Required]
        [DynamicStringLength(typeof(MapTenantConsts), nameof(MapTenantConsts.MaxCodeLength))]
        public string Code { get; set; }

        [DynamicStringLength(typeof(MapTenantConsts), nameof(MapTenantConsts.MaxMapCodeLength))]
        public string MapCode { get; set; }

        public CreateMapTenantDto()
        {

        }

        public CreateMapTenantDto(Guid tenantId, string tenantName, string code, string mapCode)
        {
            TenantId = tenantId;
            TenantName = tenantName;
            Code = code;
            MapCode = mapCode;
        }
    }
}
