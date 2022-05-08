using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.MinId
{
    public class MinIdTokenDto : AuditedEntityDto<Guid>
    {
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// BizType
        /// </summary>
        public string BizType { get; set; }

        /// <summary>
        /// Remark
        /// </summary>
        public string Remark { get; set; }
    }
}
