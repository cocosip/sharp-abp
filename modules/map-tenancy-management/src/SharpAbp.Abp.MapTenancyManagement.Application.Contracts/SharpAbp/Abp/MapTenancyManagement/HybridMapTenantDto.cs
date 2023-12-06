using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class HybridMapTenantDto : ExtensibleEntityDto<Guid>, IHasConcurrencyStamp
    {
        /// <summary>
        /// �⻧Id
        /// </summary>
        public Guid TenantId { get; set; }

        /// <summary>
        /// �⻧����
        /// </summary>
        public string TenantName { get; set; }
        public string Code { get; set; }
        public string MapCode { get; set; }

        public string ConcurrencyStamp { get; set; }
 
    }
}