using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    public class SecurityCredentialInfoPagedRequestDto : PagedAndSortedResultRequestDto
    {
        public string Identifier { get; set; }
        public string KeyType { get; set; }
        public string BizType { get; set; }
        public DateTime? ExpiresMin { get; set; }
        public DateTime? ExpiresMax { get; set; }
    }
}
