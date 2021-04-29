using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class UpdateMapTenantDto
    {
        [Required]
        [DynamicStringLength(typeof(MapTenantConsts), nameof(MapTenantConsts.MaxCodeLength))]
        public string Code { get; set; }

        [Required]
        public Guid TenantId { get; set; }

        [DynamicStringLength(typeof(MapTenantConsts), nameof(MapTenantConsts.MaxMapCodeLength))]
        public string MapCode { get; set; }

        public UpdateMapTenantDto()
        {

        }

        public UpdateMapTenantDto(string code, Guid tenantId, string mapCode)
        {
            Code = code;
            TenantId = tenantId;
            MapCode = mapCode;
        }
    }
}
