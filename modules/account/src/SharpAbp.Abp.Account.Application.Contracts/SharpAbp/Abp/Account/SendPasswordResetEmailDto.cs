using System.ComponentModel.DataAnnotations;

namespace SharpAbp.Abp.Account
{
    public class SendPasswordResetEmailDto
    {
        [Required]
        public string AppName { get; set; }

        public string ReturnUrl { get; set; }

        public string ReturnUrlHash { get; set; }
    }
}
