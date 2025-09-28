using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.TenantManagement;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.MapTenancyManagement
{
    /// <summary>
    /// Data transfer object for updating an existing map tenant with validation constraints.
    /// Contains all required information to modify a tenant mapping relationship.
    /// </summary>
    public class UpdateMapTenantDto : ExtensibleEntityDto
    {
        /// <summary>
        /// Gets or sets the tenant identifier to be associated with this map tenant
        /// </summary>
        /// <value>The unique identifier of the tenant; required field</value>
        [Required]
        public Guid TenantId { get; set; }

        /// <summary>
        /// Gets or sets the display name of the tenant
        /// </summary>
        /// <value>The human-readable name of the tenant; required field with maximum length constraint</value>
        [Required]
        [DynamicStringLength(typeof(TenantConsts), nameof(TenantConsts.MaxNameLength))]
        public string TenantName { get; set; }

        /// <summary>
        /// Gets or sets the unique code associated with the tenant
        /// </summary>
        /// <value>The tenant's unique identification code; required field with maximum length constraint</value>
        [Required]
        [DynamicStringLength(typeof(MapTenantConsts), nameof(MapTenantConsts.MaxCodeLength))]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the map code for external system integration
        /// </summary>
        /// <value>The external mapping code; optional field with maximum length constraint</value>
        [DynamicStringLength(typeof(MapTenantConsts), nameof(MapTenantConsts.MaxMapCodeLength))]
        public string MapCode { get; set; }

        /// <summary>
        /// Initializes a new instance of the UpdateMapTenantDto class with default values
        /// </summary>
        public UpdateMapTenantDto()
        {

        }

        /// <summary>
        /// Initializes a new instance of the UpdateMapTenantDto class with the specified parameters
        /// </summary>
        /// <param name="tenantId">The unique identifier of the tenant</param>
        /// <param name="tenantName">The display name of the tenant</param>
        /// <param name="code">The unique code associated with the tenant</param>
        /// <param name="mapCode">The map code for external system integration</param>
        public UpdateMapTenantDto(Guid tenantId, string tenantName, string code, string mapCode)
        {
            TenantId = tenantId;
            TenantName = tenantName;
            Code = code;
            MapCode = mapCode;
        }
    }
}
