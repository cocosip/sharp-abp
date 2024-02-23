using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.CryptoVault
{
    public class CreateSM2CredsDto
    {
        [Required]
        [DynamicStringLength(typeof(SM2CredsConsts), nameof(SM2CredsConsts.MaxCurveLength))]
        public string Curve { get; set; }

        [Required]
        public string PublicKey { get; set; }

        [Required]
        public string PrivateKey { get; set; }
    }
}
