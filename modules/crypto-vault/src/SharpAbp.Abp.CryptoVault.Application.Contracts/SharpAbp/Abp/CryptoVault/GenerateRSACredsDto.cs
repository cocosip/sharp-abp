using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SharpAbp.Abp.CryptoVault
{
    public class GenerateRSACredsDto : IValidatableObject
    {
        [Required]
        public int Size { get; set; }

        [Required]
        public int Count { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Size != 1024 && Size != 2048 && Size != 3072 && Size != 4096)
            {
                yield return new ValidationResult("Invalid key size");
            }

            if (Count <= 0 || Count > 100)
            {
                yield return new ValidationResult("Invalid key count, should greater than 0 and less than 100");
            }
        }
    }
}
