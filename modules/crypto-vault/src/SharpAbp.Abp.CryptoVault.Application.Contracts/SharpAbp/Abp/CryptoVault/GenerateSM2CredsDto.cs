using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SharpAbp.Abp.CryptoVault
{
    public class GenerateSM2CredsDto : IValidatableObject
    {

        [Required]
        public string Curve { get; set; }

        [Required]
        public int Count { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Count <= 0 || Count > 100)
            {
                yield return new ValidationResult("Invalid key count, should greater than 0 and less than 100");
            }
        }
    }
}
