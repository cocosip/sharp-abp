using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class UpdateMapTenantDto : EntityDto<Guid>
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

        public UpdateMapTenantDto(Guid id, string code, Guid tenantId, string mapCode)
        {
            Id = id;
            Code = code;
            TenantId = tenantId;
            MapCode = mapCode;
        }
    }
}
