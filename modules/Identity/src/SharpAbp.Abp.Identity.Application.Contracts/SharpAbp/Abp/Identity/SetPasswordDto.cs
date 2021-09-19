using System.ComponentModel.DataAnnotations;

namespace SharpAbp.Abp.Identity
{
    public class SetPasswordDto
    {
        [Required]
        public string NewPassword { get; set; }
    }
}
