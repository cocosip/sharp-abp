using System.ComponentModel.DataAnnotations;

using Volo.Abp.Auditing;

namespace SharpAbp.Abp.Identity
{
    public class SetPasswordDto
    {
        [Required]
        [DisableAuditing]
        public string NewPassword { get; set; }
    }
}
