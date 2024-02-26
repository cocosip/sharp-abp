using SharpAbp.Abp.Crypto.RSA;

namespace SharpAbp.Abp.TransformSecurity
{
    public class AbpTransformSecurityRSAOptions
    {
        /// <summary>
        /// Key size, default: 2048
        /// </summary>
        public int KeySize { get; set; } = 2048;

        /// <summary>
        /// Padding
        /// </summary>
        public string Padding { get; set; } = RSAPaddingNames.PKCS1Padding;
    }
}
