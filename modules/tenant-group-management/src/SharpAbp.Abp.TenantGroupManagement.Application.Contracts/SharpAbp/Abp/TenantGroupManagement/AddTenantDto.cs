using System;
using System.ComponentModel.DataAnnotations;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class AddTenantDto
    {
        [Required]
        public Guid TenantId { get; set; }
    }
}
