using System.ComponentModel.DataAnnotations;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    public class CreateSecurityCredentialInfoDto
    {
        [Required]
        public string BizType { get; set; }
    }
}
