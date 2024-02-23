using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SharpAbp.Abp.CryptoVault
{
    public class CreateRSACredsDto : IValidatableObject
    {
        [Required]
        public int Size { get; set; }

        [Required]
        public string PublicKey { get; set; }

        [Required]
        public string PrivateKey { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Size != 1024 && Size != 2048 && Size != 3072 && Size != 4096)
            {
                yield return new ValidationResult("Invalid key size");
            }
        }
    }
}
